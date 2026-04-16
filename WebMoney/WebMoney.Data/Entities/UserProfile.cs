namespace WebMoney.Persistence.Entities;

public class UserProfile : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<CardUserProfile> CardUserProfiles { get; set; }
    public IdentityDocument? IdentityDocument { get; set; }
}