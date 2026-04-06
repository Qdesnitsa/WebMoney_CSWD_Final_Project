namespace WebMoney.Persistence.Entities;

public class Card : BaseEntity
{
    public string Url { get; set; }
    public string Number { get; set; }
    public List<Transaction> Transactions { get; set; }
}