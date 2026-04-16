using WebMoney.Data.Enum.Card;
using WebMoney.Enum;

namespace WebMoney.Persistence.Entities;

public class Card : BaseEntity
{
    public string Number { get; set; }
    public CurrencyCode CurrencyCode { get; set; }
    public CardStatus CardStatus { get; set; }
    public string HashedPinCode { get; set; }
    public ICollection<CardUserProfile> CardUserProfiles { get; set; } = new HashSet<CardUserProfile>();
    public ICollection<Transaction> Transactions { get; set; }
    public DateOnly PeriodOfValidity { get; set; }
    public decimal Balance { get; set; }
}