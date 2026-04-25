using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application;
using WebMoney.Application.Transactions;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

public class TransactionController(ICardService cardService, IMediator mediator) : Controller
{
    [HttpGet]
    public IActionResult Transaction([FromQuery] int? cardId, [FromQuery] DateOnly? periodFrom,
        [FromQuery] DateOnly? periodTo)
    {
        var username = HttpContext.Session.GetString(SessionKeys.USERNAME);
        if (string.IsNullOrWhiteSpace(username))
        {
            return RedirectToAction(nameof(SignInController.SignIn), "SignIn");
        }

        if (!cardId.HasValue)
        {
            return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
        }

        var periodKeysPresent = Request.Query.ContainsKey("periodFrom") || Request.Query.ContainsKey("periodTo");
        var query = new GetTransactionStatementQuery(cardId.Value, periodFrom, periodTo, periodKeysPresent);

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
            validationModel.CardNumber = card.Card.Number;

            return View(validationModel);
        }

        var model = new TransactionViewModel
        {
            CardId = result.CardId,
            CardNumber = result.CardNumber,
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