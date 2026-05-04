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
    public IActionResult Transaction([FromQuery] TransactionViewModel model)
    {
        if (model.CardId <= 0)
        {
            return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
        }

        var userId = User.WebMoneyUserId()!.Value;
        if (!cardService.UserIsCardParticipant(userId, model.CardId))
        {
            return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
        }

        if (!ModelState.IsValid)
        {
            var card = cardService.GetById(model.CardId);
            model.CardNumberMasked = CardNumberMask.Mask(card.Card?.Number);
            return View(model);
        }

        var periodKeysPresent = PeriodKeysInQuery(Request);
        var query = new GetTransactionStatementQuery(model.CardId, userId, model.PeriodFrom, model.PeriodTo,
            periodKeysPresent);

        var result = mediator.SendSync(query);

        var viewModel = new TransactionViewModel
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
            viewModel.Alerts.AddRange(result.Errors.Select(e => e.Message));
        }

        return View(viewModel);
    }

    private static bool PeriodKeysInQuery(HttpRequest request) =>
        request.Query.Keys.Any(static k =>
            string.Equals(k, "PeriodFrom", StringComparison.OrdinalIgnoreCase)
            || string.Equals(k, "PeriodTo", StringComparison.OrdinalIgnoreCase));
}
