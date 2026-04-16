using WebMoney.Data.Persistence;
using WebMoney.Persistence.Entities;

namespace WebMoney.Persistence;

public interface IUserProfileRepository : IBaseRepository<UserProfile>
{
    bool EmailExists(string normalizedEmail);
    UserProfile? FindByEmail(string normalizedEmail);
}