using Microsoft.AspNetCore.Mvc;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

public class DepositController(ICardService cardService, IDepositTransactionService depositTransactionService)
    : Controller
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
            CardNumber = result.Card.Number
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
            || string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionKeys.USEREMAIL)))
        {
            return RedirectToAction(nameof(SignInController.SignIn),
                nameof(SignInController).Replace("Controller", ""));
        }

        if (model.Amount <= 0)
        {
            ModelState.AddModelError(nameof(model.Amount), "Сумма должна быть больше нуля");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = depositTransactionService.SubmitNewDeposit(model.CardId, useremail!, model.Amount);

        if (!result.Success)
        {
            model.Alerts.AddRange(result.Errors.Select(e => e.Message));
            return View(model);
        }

        return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
    }
}