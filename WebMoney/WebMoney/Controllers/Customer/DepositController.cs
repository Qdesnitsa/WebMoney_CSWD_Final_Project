using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application;
using WebMoney.Application.Deposits;
using WebMoney.Auth;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

[Authorize(Policy = AuthPolicies.UserOnly)]
public class DepositController(ICardService cardService, IMediator mediator) : Controller
{
    [HttpGet]
    public IActionResult NewDeposit([FromQuery] int? cardId)
    {
        if (!cardId.HasValue)
        {
            return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
        }

        var userId = User.WebMoneyUserId()!.Value;
        var result = cardService.GetById(cardId.Value);
        if (!result.Success || result.Card is null)
        {
            return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
        }

        if (!cardService.UserIsCardParticipant(userId, cardId.Value))
        {
            return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
        }

        var model = new NewDepositViewModel
        {
            CardId = cardId.Value,
            CardNumberMasked = CardNumberMask.Mask(result.Card.Number)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult NewDeposit(NewDepositViewModel model)
    {
        var userId = User.WebMoneyUserId()!;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var command = new PrepareNewDepositCommand(model.CardId, userId.Value, model.Amount);

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