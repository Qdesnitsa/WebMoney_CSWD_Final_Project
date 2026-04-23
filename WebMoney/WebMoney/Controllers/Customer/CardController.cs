using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application.Cards;
using WebMoney.Infrastructure.Constants;
using WebMoney.ModelTransfer;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

public class CardController(ICardService cardService, IValidator<PrepareNewCardCommand> prepareNewCardValidator)
    : Controller
{
    public IActionResult Card()
    {
        var username = HttpContext.Session.GetString(SessionKeys.USERNAME);
        var useremail = HttpContext.Session.GetString(SessionKeys.USEREMAIL);
        if (string.IsNullOrWhiteSpace(username))
        {
            return RedirectToAction(nameof(SignInController.SignIn),
                nameof(SignInController).Replace("Controller", ""));
        }

        var cardViewModel = new CardViewModel
        {
            Cards = cardService.GetCardsByUserEmail(useremail).Select(c => new CardViewModel
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
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionKeys.USERNAME)))
        {
            return RedirectToAction(nameof(SignInController.SignIn),
                nameof(SignInController).Replace("Controller", ""));
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
        var userEmail = HttpContext.Session.GetString(SessionKeys.USEREMAIL);
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionKeys.USERNAME)) ||
            userEmail == null)
        {
            return RedirectToAction(nameof(SignInController.SignIn),
                nameof(SignInController).Replace("Controller", ""));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = userEmail.Trim().ToLowerInvariant();
        var command = new PrepareNewCardCommand(
            normalizedEmail,
            model.CardNumber,
            model.CurrencyCode,
            model.DailyLimit,
            model.MonthlyLimit,
            model.PerOperationLimit,
            model.PinCode);

        var validationResult = prepareNewCardValidator.Validate(command);
        if (!validationResult.IsValid)
        {
            foreach (var err in validationResult.Errors)
            {
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
            }

            return View(model);
        }

        var input = new NewCardInput
        {
            CardNumber = command.CardNumber,
            CurrencyCode = command.CurrencyCode,
            DailyLimit = command.DailyLimit,
            MonthlyLimit = command.MonthlyLimit,
            PerOperationLimit = command.PerOperationLimit,
            PinCode = command.PinCode
        };

        var result = cardService.PrepareNewCard(command.NormalizedEmail, input);

        if (!result.Success)
        {
            foreach (var (_, message) in result.Errors)
            {
                if (!string.IsNullOrWhiteSpace(message) && !model.Alerts.Contains(message))
                {
                    model.Alerts.Add(message);
                }
            }

            return View(model);
        }

        return RedirectToAction(nameof(Card));
    }
}