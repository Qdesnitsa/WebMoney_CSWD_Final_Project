using WebMoney.Enum;

namespace WebMoney.Persistence.Entities;

public class Transaction : BaseEntity
{
    public DateTime DateTime { get; set; }
    public TransactionType TransactionType { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public CurrencyCode CurrencyCode { get; set; }
    public string RRN { get; set; }
    public string Counterparty { get; set; }
    public decimal Amount { get; set; }
}