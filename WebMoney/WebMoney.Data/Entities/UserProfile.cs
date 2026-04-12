namespace WebMoney.Persistence.Entities;

public class UserProfile : User
{
    public ICollection<Card> Cards { get; set; }
    public IdentityDocument? IdentityDocument { get; set; }
}