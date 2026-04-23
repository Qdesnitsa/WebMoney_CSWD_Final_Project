using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application.Deposits;
using WebMoney.Infrastructure.Constants;
using WebMoney.ModelTransfer;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

public class DepositController(
    ICardService cardService,
    IDepositTransactionService depositTransactionService,
    IValidator<PrepareNewDepositCommand> submitNewDepositValidator) : Controller
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

        var validationResult = submitNewDepositValidator.Validate(command);
        if (!validationResult.IsValid)
        {
            foreach (var err in validationResult.Errors)
            {
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
            }

            return View(model);
        }

        var result = depositTransactionService.SubmitNewDeposit(
            command.CardId,
            command.NormalizedEmail,
            command.Amount);

        if (!result.Success)
        {
            model.Alerts.AddRange(result.Errors.Select(e => e.Message));
            return View(model);
        }

        return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
    }
}
