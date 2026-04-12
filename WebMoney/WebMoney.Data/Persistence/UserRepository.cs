using WebMoney.Data;
using WebMoney.Persistence.Entities;

namespace WebMoney.Persistence;

public class UserRepository(WebContext webContext) : IUserRepository
{
    public bool EmailExists(string normalizedEmail) => webContext.Users.Any(u => u.Email == normalizedEmail);

    public void Create(UserProfile user)
    {
        webContext.Users.Add(user);
        webContext.SaveChanges();
    }

    public UserProfile FindByEmail(string normalizedEmail) => 
        webContext.Users.FirstOrDefault(u => u.Email == normalizedEmail);

    public IQueryable<UserProfile> GetAllUsers() => webContext.Users;
}