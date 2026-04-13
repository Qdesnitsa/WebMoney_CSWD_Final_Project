namespace WebMoney.Persistence.Entities;

public class UserProfile : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<Card> Cards { get; set; }
    public IdentityDocument? IdentityDocument { get; set; }
}