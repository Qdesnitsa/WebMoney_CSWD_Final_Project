using WebMoney.Data.Enum;

namespace WebMoney.Persistence.Entities;

public class Card : BaseEntity
{
    public string Number { get; set; }
    public CurrencyCode CurrencyCode { get; set; }
    public CardStatus CardStatus { get; set; }
    public string HashedPinCode { get; set; }
    public DateOnly PeriodOfValidity { get; set; }
    public decimal Balance { get; set; }
    public virtual ICollection<CardUserProfile> CardUserProfiles { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; }
}
