namespace WebMoney.Persistence.Entities;

public class IdentityDocument : BaseEntity
{
    public virtual UserProfile UserProfile { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DocumentIDNumber { get; set; }
    public string DocumentPhotoLink { get; set; }
}