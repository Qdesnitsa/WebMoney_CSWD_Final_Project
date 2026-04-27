using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application;
using WebMoney.Application.Cards;
using WebMoney.Auth;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

[Authorize(Policy = AuthPolicies.UserOnly)]
public class CardController(ICardService cardService, IMediator mediator) : Controller
{
    public IActionResult Card()
    {
        var username = User.WebMoneyUserName()!;
        var userId = User.WebMoneyUserId()!;

        var cardViewModel = new CardViewModel
        {
            Cards = cardService.GetCardsByUserId(userId.Value).Select(c => new CardViewModel
            {
                Id = c.Id,
                Number = c.Number,
                UserName = username,
                ValidThru = c.PeriodOfValidity.ToString(),
                UserEmail = c.CreatedBy,
                Balance = c.Balance,
                CurrencyCode = c.CurrencyCode.ToString()
            }).ToList()
        };

        return View(cardViewModel);
    }

    [HttpGet]
    public IActionResult NewCard()
    {
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

        PrepareNewCardResult result;
        try
        {
            result = mediator.SendSync(command);
        }
        catch (ValidationException ex)
        {
            foreach (var err in ex.Errors)
            {
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
            }

            return View(model);
        }
        
        if (!result.Success)
        {
            model.Alerts.AddRange(result.Errors.Select(e => e.Message));
            return View(model);
        }

        return RedirectToAction(nameof(Card));
    }
}
