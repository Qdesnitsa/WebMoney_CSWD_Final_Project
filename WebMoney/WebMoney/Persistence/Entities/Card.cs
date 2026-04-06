namespace WebMoney.Persistence.Entities;

using Type = WebMoney.Enum.Card.Type;

public class Card : BaseEntity
{
    public string Url { get; set; }
    public Type Type { get; set; }
    public string Number { get; set; }
    public DateTime Expiration { get; set; }
    public List<Transaction> Transactions { get; set; }
}