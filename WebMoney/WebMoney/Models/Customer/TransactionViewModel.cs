using WebMoney.Enum;

namespace WebMoney.Models;

public class TransactionViewModel
{
    public int CardId { get; set; }
    public string CardNumber { get; set; }
    public DateOnly? PeriodFrom { get; set; }
    public DateOnly? PeriodTo { get; set; }
    public DateTime DateTime { get; set; }
    public TransactionType TransactionType { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public CurrencyCode CurrencyCode { get; set; }
    public string Counterparty { get; set; }
    public decimal Amount { get; set; }
    public List<TransactionViewModel> Transactions { get; set; } = new();
}