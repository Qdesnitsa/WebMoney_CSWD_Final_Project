using WebMoney.Data.Enum.Card;
using WebMoney.Enum;

namespace WebMoney.Persistence.Entities;

public class Card : BaseEntity
{
    public string Number { get; set; }
    public CurrencyCode CurrencyCode { get; set; }
    public int? CardLimitId { get; set; }
    public CardLimit? CardLimit { get; set; }
    public CardStatus CardStatus { get; set; }
    public string HashedPinCode { get; set; }
    public ICollection<UserProfile> UserProfiles { get; set; } = new HashSet<UserProfile>();
    public ICollection<Transaction> Transactions { get; set; }
    public DateOnly PeriodOfValidity { get; set; }
}