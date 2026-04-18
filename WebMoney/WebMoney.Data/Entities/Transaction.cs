using WebMoney.Enum;
using TransactionStatus = WebMoney.Enum.TransactionStatus;

namespace WebMoney.Persistence.Entities;

public class Transaction : BaseEntity
{
    public int CardId { get; set; }
    public virtual Card Card { get; set; }
    public TransactionType TransactionType { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public int CounterpartyId { get; set; }
    public virtual Counterparty Counterparty { get; set; }
    public decimal Amount { get; set; }
}