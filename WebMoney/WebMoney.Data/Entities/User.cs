using WebMoney.Data.Enum;

namespace WebMoney.Data.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public Role Role { get; set; }
    public virtual UserProfile? UserProfile { get; set; }
    public virtual ICollection<CardUserProfile>? CardUserProfiles { get; set; }
}
