using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories;

public class UserRepository(WebContext webContext) : BaseRepository<User>(webContext), IUserRepository
{
    public bool EmailExists(string normalizedEmail) =>
        webContext.Users.Any(u => u.Email == normalizedEmail);

    public User? FindByEmail(string normalizedEmail) =>
        webContext.Users.FirstOrDefault(u => u.Email == normalizedEmail);

    public void CreateWithProfile(UserProfile profile)
    {
        webContext.UsersProfiles.Add(profile);
        webContext.SaveChanges();
    }

    public void SaveChanges() => webContext.SaveChanges();
}