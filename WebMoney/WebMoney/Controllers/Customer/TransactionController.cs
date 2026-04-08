using Microsoft.AspNetCore.Mvc;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Persistence.Storage;
using WebMoney.Services;

namespace WebMoney.Controllers;

public class TransactionController(ITransactionService transactionService) : Controller
{
    [HttpGet]
    public IActionResult Show([FromQuery] int? cardId, [FromQuery] DateOnly? periodFrom, [FromQuery] DateOnly? periodTo)
    {
        var username = HttpContext.Session.GetString(SessionKeys.USERNAME);
        if (string.IsNullOrWhiteSpace(username))
        {
            return RedirectToAction(nameof(SignInController.SignIn), "SignIn");
        }

        if (!cardId.HasValue)
        {
            return RedirectToAction(nameof(CardController.Show), "Card");
        }

        var periodKeysPresent = Request.Query.ContainsKey("periodFrom") || Request.Query.ContainsKey("periodTo");
        var result = transactionService.GetStatement(cardId.Value, periodFrom, periodTo, periodKeysPresent);

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
                DateTime = t.DateTime,
                TxnType = t.TxnType,
                Status = t.Status,
                CurrencyCode = t.CurrencyCode,
                RRN = t.RRN,
                Counterparty = t.Counterparty,
                Amount = t.Amount
            }).ToList()
        };
        return View("/Views/Customer/Transaction.cshtml", model);
    }
}