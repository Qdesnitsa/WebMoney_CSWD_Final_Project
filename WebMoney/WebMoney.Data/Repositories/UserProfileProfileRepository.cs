using Microsoft.EntityFrameworkCore;
using WebMoney.Data;
using WebMoney.Data.Persistence;
using WebMoney.Persistence.Entities;

namespace WebMoney.Persistence;

public class UserProfileProfileRepository(WebContext webContext)
    : BaseRepository<UserProfile>(webContext), IUserProfileRepository
{
    public bool EmailExists(string normalizedEmail) =>
        webContext.UsersProfiles.Any(u => u.User.Email == normalizedEmail);

    public UserProfile FindByEmail(string normalizedEmail) =>
        webContext.UsersProfiles
            .Include(u => u.User)
            .FirstOrDefault(u => u.User.Email == normalizedEmail);
}