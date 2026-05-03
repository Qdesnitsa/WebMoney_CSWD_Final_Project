namespace WebMoney.Data.Entities;

public class IdentityDocument : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DocumentIDNumber { get; set; }
    public string DocumentPhotoLink { get; set; }
    public virtual UserProfile UserProfile { get; set; }
}