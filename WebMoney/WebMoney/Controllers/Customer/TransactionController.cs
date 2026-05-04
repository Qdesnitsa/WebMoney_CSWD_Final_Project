using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application;
using WebMoney.Application.Transactions;
using WebMoney.Auth;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

[Authorize(Policy = AuthPolicies.UserOnly)]
public class TransactionController(ICardService cardService, IMediator mediator) : Controller
{
    [HttpGet]
    public IActionResult Transaction([FromQuery] int? cardId, [FromQuery] DateOnly? periodFrom,
        [FromQuery] DateOnly? periodTo)
    {
        if (!cardId.HasValue)
        {
            return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
        }

        var userId = User.WebMoneyUserId()!.Value;
        if (!cardService.UserIsCardParticipant(userId, cardId.Value))
        {
            return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
        }

        var periodKeysPresent = Request.Query.ContainsKey("periodFrom") || Request.Query.ContainsKey("periodTo");
        var query = new GetTransactionStatementQuery(cardId.Value, userId, periodFrom, periodTo, periodKeysPresent);

        TransactionStatementResult result;
        try
        {
            result = mediator.SendSync(query);
        }
        catch (ValidationException ex)
        {
            var validationModel = new TransactionViewModel
            {
                CardId = cardId.Value,
                PeriodFrom = periodFrom,
                PeriodTo = periodTo
            };

            foreach (var err in ex.Errors)
            {
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
            }

            var card = cardService.GetById(cardId.Value);
            validationModel.CardNumberMasked = CardNumberMask.Mask(card.Card?.Number);

            return View(validationModel);
        }

        var model = new TransactionViewModel
        {
            CardId = result.CardId,
            CardNumberMasked = CardNumberMask.Mask(result.CardNumber),
            PeriodFrom = result.PeriodFrom,
            PeriodTo = result.PeriodTo,
            ShowEmptyPeriodMessage = result.ShowEmptyPeriodMessage,
            Transactions = result.Transactions.Select(t => new TransactionViewModel
            {
                DateTime = t.CreatedAt,
                TransactionType = t.TransactionType,
                TransactionStatus = t.TransactionStatus,
                CurrencyCode = t.Card.CurrencyCode,
                Counterparty = t.Counterparty.Name,
                Amount = t.Amount
            }).ToList()
        };

        if (!result.Success)
        {
            model.Alerts.AddRange(result.Errors.Select(e => e.Message));
        }

        return View(model);
    }
}