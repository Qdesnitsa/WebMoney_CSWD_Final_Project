using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application;
using WebMoney.Application.Cards;
using WebMoney.Auth;
using WebMoney.Models;
using WebMoney.Models.EditCardUsers;
using WebMoney.Data.Entities;
using WebMoney.Services;

namespace WebMoney.Controllers;

[Authorize(Policy = AuthPolicies.UserOnly)]
public class CardController(ICardService cardService, IMediator mediator) : Controller
{
    private const bool AlwaysShowPaymentOperations = true;

    public IActionResult Card()
    {
        var username = User.WebMoneyUserName()!;
        var userId = User.WebMoneyUserId()!;
        var cardsForUser = cardService.GetCardsForUser(userId.Value);
        if (cardsForUser is null)
        {
            return RedirectToAction(nameof(AuthController.SignIn), nameof(AuthController).Replace("Controller", ""));
        }

        var cardViewModel = new CardViewModel
        {
            Cards = cardsForUser.Select(cardForUser => new CardViewModel
                {
                    Id = cardForUser.CardId,
                    Number = cardForUser.MaskedNumber,
                    UserName = username,
                    ValidThru = cardForUser.ValidThru,
                    UserEmail = cardForUser.CreatedBy,
                    Balance = cardForUser.Balance,
                    CurrencyCode = cardForUser.CurrencyCode,
                    ShowPaymentOperations = AlwaysShowPaymentOperations,
                    ShowUserManagement = cardForUser.ShowUserManagement,
                    IsOwner = cardForUser.IsOwner,
                })
                .ToList()
        };

        return View(cardViewModel);
    }

    [HttpGet]
    public IActionResult NewCard()
    {
        var userId = User.WebMoneyUserId()!.Value;
        var cardsForUser = cardService.GetCardsForUser(userId);
        if (cardsForUser is null)
        {
            return RedirectToAction(nameof(AuthController.SignIn), nameof(AuthController).Replace("Controller", ""));
        }

        if (cardsForUser.Count > 0 && !cardsForUser.Any(c => c.IsOwner))
        {
            return RedirectToAction(nameof(Card));
        }

        var model = new NewCardViewModel
        {
            CardNumber = cardService.GenerateNotExistingCardNumber(),
            PeriodOfValidity = cardService.DefaultPeriodOfValidity()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult NewCard(NewCardViewModel model)
    {
        var userId = User.WebMoneyUserId()!;
        var cardsForUser = cardService.GetCardsForUser(userId.Value);
        if (cardsForUser is null)
        {
            return RedirectToAction(nameof(AuthController.SignIn), nameof(AuthController).Replace("Controller", ""));
        }

        if (cardsForUser.Count > 0 && !cardsForUser.Any(c => c.IsOwner))
        {
            return RedirectToAction(nameof(Card));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var command = new PrepareNewCardCommand(
            userId.Value,
            model.CardNumber,
            model.CurrencyCode,
            model.DailyLimit,
            model.MonthlyLimit,
            model.PerOperationLimit,
            model.PinCode);

        var result = mediator.SendSync(command);

        if (!result.Success)
        {
            model.Alerts.AddRange(result.Errors.Select(e => e.Message));
            return View(model);
        }

        return RedirectToAction(nameof(Card));
    }

    [HttpGet]
    public IActionResult AddUser([FromQuery] int? cardId)
    {
        if (!cardId.HasValue)
        {
            return RedirectToAction(nameof(Card));
        }

        var userId = User.WebMoneyUserId()!.Value;
        var cardResult = cardService.GetCardWithUsersAndCardLimitsById(cardId.Value);
        if (!cardResult.Success || !cardService.UserMayManageCardUsers(userId, cardId.Value))
        {
            return RedirectToAction(nameof(Card));
        }

        var model = new AddCardUserViewModel
        {
            CardId = cardId.Value,
            CardNumberMasked = CardNumberMask.Mask(cardResult.Card!.Number),
            CurrentUserMayManageCardUsers = cardService.UserMayManageCardUsers(userId, cardId.Value)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddUser(AddCardUserViewModel model)
    {
        var userId = User.WebMoneyUserId()!;
        if (!ModelState.IsValid)
        {
            var cardResult = cardService.GetCardWithUsersAndCardLimitsById(model.CardId);
            model.CardNumberMasked = CardNumberMask.Mask(cardResult.Card?.Number);
            model.CurrentUserMayManageCardUsers = cardService.UserMayManageCardUsers(userId.Value, model.CardId);
            return View(model);
        }

        var result = cardService.AddUserToCard(
            userId.Value,
            model.CardId,
            model.Email,
            model.DailyLimit,
            model.MonthlyLimit,
            model.PerOperationLimit,
            model.CanManageUsers);

        if (!result.Success)
        {
            model.CardNumberMasked = CardNumberMask.Mask(result.Card?.Number);
            model.CurrentUserMayManageCardUsers = cardService.UserMayManageCardUsers(userId.Value, model.CardId);
            model.Alerts.AddRange(result.Errors.Select(e => e.Message));
            return View(model);
        }

        return RedirectToAction(nameof(EditUsers), new { id = model.CardId });
    }

    [HttpGet]
    public IActionResult EditUsers(int? cardId)
    {
        if (!cardId.HasValue)
        {
            return RedirectToAction(nameof(Card));
        }

        var userId = User.WebMoneyUserId()!.Value;
        var cardResult = cardService.GetCardWithUsersAndCardLimitsById(cardId.Value);
        if (!cardResult.Success || !cardService.UserMayManageCardUsers(userId, cardId.Value))
        {
            return RedirectToAction(nameof(Card));
        }

        var model = BuildEditUsersModel(cardResult.Card, userId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditUsers(int cardId, EditUsersViewModel.UpdateCardUserAccessViewModel model)
    {
        if (cardId != model.CardId)
        {
            return RedirectToAction(nameof(Card));
        }

        var userId = User.WebMoneyUserId()!;
        var fetchedCard = cardService.GetCardWithUsersAndCardLimitsById(cardId);
        if (!fetchedCard.Success)
        {
            return RedirectToAction(nameof(Card));
        }

        var targetCardUserProfile =
            fetchedCard.Card.CardUserProfiles?.FirstOrDefault(x => x.Id == model.CardUserProfileId);
        var targetIsOwner = targetCardUserProfile is not null &&
                            string.Equals(targetCardUserProfile.User?.Email, fetchedCard.Card.CreatedBy,
                                StringComparison.OrdinalIgnoreCase);

        bool? grantUsers = null;
        if (cardService.UserMayManageCardUsers(userId.Value, cardId) && !targetIsOwner &&
            Request.HasFormContentType &&
            Request.Form.TryGetValue("CanManageUsers", out var canManageUsersForm))
        {
            grantUsers = canManageUsersForm.Any(v =>
                string.Equals(v, "true", StringComparison.OrdinalIgnoreCase));
        }

        var result = cardService.UpdateUserAccess(
            userId.Value,
            model.CardId,
            model.CardUserProfileId,
            model.DailyLimit,
            model.MonthlyLimit,
            model.PerOperationLimit,
            grantUsers);
        if (!result.Success)
        {
            var cardResult = cardService.GetCardWithUsersAndCardLimitsById(cardId);
            if (!cardResult.Success)
            {
                return RedirectToAction(nameof(Card));
            }

            var errorModel = BuildEditUsersModel(cardResult.Card, userId.Value);
            errorModel.Alerts.AddRange(result.Errors.Select(e => e.Message));
            return View(errorModel);
        }

        return RedirectToAction(nameof(EditUsers), new { id = model.CardId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveUser(int id, EditUsersViewModel.RemoveCardUserAccessViewModel model)
    {
        if (id != model.CardId)
        {
            return RedirectToAction(nameof(Card));
        }

        var userId = User.WebMoneyUserId()!;
        var result = cardService.RemoveUserAccess(userId.Value, model.CardId, model.CardUserProfileId);
        if (!result.Success)
        {
            var cardResult = cardService.GetCardWithUsersAndCardLimitsById(id);
            if (!cardResult.Success)
            {
                return RedirectToAction(nameof(Card));
            }

            var errorModel = BuildEditUsersModel(cardResult.Card, userId.Value);
            errorModel.Alerts.AddRange(result.Errors.Select(e => e.Message));
            return View(nameof(EditUsers), errorModel);
        }

        return RedirectToAction(nameof(EditUsers), new { id = model.CardId });
    }

    private static EditUsersViewModel BuildEditUsersModel(Card card, int currentUserId)
    {
        var currentUserProfile = card.CardUserProfiles.FirstOrDefault(c => c.UserId == currentUserId);
        var userFromProfile = currentUserProfile?.User;
        var currentUserCanManageUsers =
            userFromProfile is not null &&
            CardPermissions.MayManageCardUsers(userFromProfile, card);

        return new EditUsersViewModel
        {
            CardId = card.Id,
            CardNumberMasked = CardNumberMask.Mask(card.Number),
            CurrentUserMayManageUsers = currentUserCanManageUsers,
            Users = card.CardUserProfiles
                .OrderBy(cup => cup.User.Email)
                .Select(cup => new EditUsersViewModel.CardUserAccessViewModel
                {
                    CardUserProfileId = cup.Id,
                    UserId = cup.UserId,
                    UserEmail = cup.User.Email,
                    IsOwner = string.Equals(cup.User.Email, card.CreatedBy, StringComparison.OrdinalIgnoreCase),
                    CanManageUsers = cup.CanManageUsers,
                    DailyLimit = cup.CardLimit?.DailyLimit,
                    MonthlyLimit = cup.CardLimit?.MonthlyLimit,
                    PerOperationLimit = cup.CardLimit?.PerOperationLimit,
                })
                .ToList()
        };
    }
}