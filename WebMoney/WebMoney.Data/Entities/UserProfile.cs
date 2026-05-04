namespace WebMoney.Data.Entities;

public class UserProfile : BaseEntity
{
    public int UserId { get; set; }
    public int? IdentityDocumentId { get; set; }
    public virtual User User { get; set; }
    public virtual IdentityDocument? IdentityDocument { get; set; }
}