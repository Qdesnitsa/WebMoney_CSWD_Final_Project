using Microsoft.AspNetCore.Mvc;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

public class TransactionController(ITransactionService transactionService) : Controller
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
        var result = transactionService.GetTransactionsByCardIdForPeriod(cardId.Value, periodFrom, periodTo, periodKeysPresent);

        if (result.IsCardMissing)
        {
            return NotFound();
        }

        if (result.ErrorMessage is not null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
        }

        var model = new TransactionViewModel
        {
            CardId = result.CardId,
            CardNumber = result.CardNumber,
            PeriodFrom = result.PeriodFrom,
            PeriodTo = result.PeriodTo,
            Transactions = result.Transactions.Select(t => new TransactionViewModel
            {
                DateTime = t.CreatedAt,
                TransactionType = t.TransactionType,
                TransactionStatus = t.TransactionStatus,
                CurrencyCode = t.Card.CurrencyCode,
                Counterparty = t.Counterparty.ToString(),
                Amount = t.Amount
            }).ToList()
        };
        return View(model);
    }
}