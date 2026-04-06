using WebMoney.Enum;

namespace WebMoney.Models;

public class TransactionViewModel
{
    public int CardId { get; set; }
    public string CardNumber { get; set; }

    public DateOnly? PeriodFrom { get; set; }
    public DateOnly? PeriodTo { get; set; }
    public DateTime DateTime { get; set; }
    public TxnType TxnType { get; set; }
    public Status Status { get; set; }
    public CurrencyCode CurrencyCode { get; set; }
    public string RRN { get; set; }
    public string Counterparty { get; set; }
    public decimal Amount { get; set; }
    public List<TransactionViewModel> Transactions { get; set; } = new();
}