namespace WebMoney.Persistence.Entities;

public class IdentityDocument : BaseEntity
{
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DocumentIDNumber { get; set; }
    public string DocumentPhotoLink { get; set; }
}