using WebMoney.Enum;
using WebMoney.Views;

namespace WebMoney.Models;

public class TransactionViewModel : BasePageViewModel
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

    public bool ShowEmptyPeriodMessage { get; set; }
}