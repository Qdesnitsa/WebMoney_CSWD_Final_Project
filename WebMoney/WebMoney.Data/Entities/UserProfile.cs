namespace WebMoney.Persistence.Entities;

public class UserProfile : BaseEntity
{
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<CardUserProfile> CardUserProfiles { get; set; }
    public int? IdentityDocumentId { get; set; }
    public virtual IdentityDocument? IdentityDocument { get; set; }
}