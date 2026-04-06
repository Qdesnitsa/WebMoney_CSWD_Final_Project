using Microsoft.AspNetCore.Identity;
using WebMoney.Enum;

namespace WebMoney.Persistence.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public Role Role { get; set; }
    public List<Card> Cards { get; set; }
    public IdentityDocument IdentityDocument { get; set; }
}