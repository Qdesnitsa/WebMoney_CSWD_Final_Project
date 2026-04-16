using Microsoft.EntityFrameworkCore;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories;

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