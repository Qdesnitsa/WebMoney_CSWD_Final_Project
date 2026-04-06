using WebMoney.Enum;

namespace WebMoney.Persistence.Entities;

public class Transaction : BaseEntity
{
    public DateTime DateTime { get; set; }
    public TxnType TxnType { get; set; }
    public Status Status { get; set; }
    public CurrencyCode CurrencyCode { get; set; }
    public string RRN { get; set; }
    public string Counterparty { get; set; }
    public decimal Amount { get; set; }
}