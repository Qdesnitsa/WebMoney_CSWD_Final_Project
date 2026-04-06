using Microsoft.AspNetCore.Mvc;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Persistence.Storage;

namespace WebMoney.Controllers;

public class TransactionController(ICardStore cardStore) : Controller
{
    [HttpGet]
    public IActionResult Show([FromQuery] int? cardId, [FromQuery] DateOnly? periodFrom, [FromQuery] DateOnly? periodTo)
    {
        var username = HttpContext.Session.GetString(SessionKeys.USERNAME);
        if (string.IsNullOrWhiteSpace(username))
            return RedirectToAction("SignIn", "SignIn");

        if (!cardId.HasValue)
        {
            return RedirectToAction("Index", "Card");
        }

        var card = cardStore.GetAllCards().FirstOrDefault(c => c.Id == cardId.Value);
        if (card is null)
            return NotFound();

        var model = new TransactionViewModel
        {
            CardId = cardId.Value,
            CardNumber = card.Number,
            PeriodFrom = periodFrom,
            PeriodTo = periodTo
        };

        if (!periodFrom.HasValue || !periodTo.HasValue)
        {
            if (Request.Query.ContainsKey("periodFrom") || Request.Query.ContainsKey("periodTo"))
                ModelState.AddModelError(string.Empty, "Укажите обе даты периода.");
            return View("~/Views/Customer/Transaction.cshtml", model);
        }

        if (periodFrom > periodTo)
        {
            ModelState.AddModelError(string.Empty, "Дата «с» не может быть позже даты «по».");
            return View("~/Views/Customer/Transaction.cshtml", model);
        }

        var rangeStart = periodFrom.Value.ToDateTime(TimeOnly.MinValue);
        var rangeEnd = periodTo.Value.ToDateTime(TimeOnly.MaxValue);

        var rows = cardStore.GetTransactionsForPeriodByCard(cardId.Value, rangeStart, rangeEnd);
        model.Transactions = rows.Select(t => new TransactionViewModel
        {
            DateTime = t.DateTime,
            TxnType = t.TxnType,
            Status = t.Status,
            CurrencyCode = t.CurrencyCode,
            RRN = t.RRN,
            Counterparty = t.Counterparty,
            Amount = t.Amount
        }).ToList();

        return View("~/Views/Customer/Transaction.cshtml", model);
    }
}