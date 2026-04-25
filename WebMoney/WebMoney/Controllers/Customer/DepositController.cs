using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application;
using WebMoney.Application.Deposits;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

public class DepositController(ICardService cardService, IMediator mediator) : Controller
{
    [HttpGet]
    public IActionResult NewDeposit([FromQuery] int? cardId)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionKeys.USERNAME)))
        {
            return RedirectToAction(nameof(SignInController.SignIn),
                nameof(SignInController).Replace("Controller", ""));
        }

        if (!cardId.HasValue)
        {
            return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
        }

        var result = cardService.GetById(cardId.Value);

        var model = new NewDepositViewModel
        {
            CardId = cardId.Value,
            CardNumber = result.Card?.Number ?? string.Empty
        };

        if (!result.Success)
        {
            model.Alerts.AddRange(result.Errors.Select(e => e.Message));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult NewDeposit(NewDepositViewModel model)
    {
        var useremail = HttpContext.Session.GetString(SessionKeys.USEREMAIL);
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionKeys.USERNAME))
            || string.IsNullOrWhiteSpace(useremail))
        {
            return RedirectToAction(nameof(SignInController.SignIn),
                nameof(SignInController).Replace("Controller", ""));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = useremail.Trim().ToLowerInvariant();
        var command = new PrepareNewDepositCommand(model.CardId, normalizedEmail, model.Amount);

        PrepareNewDepositResult result;
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

        return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
    }
}
