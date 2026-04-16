using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories.Interfaces;

public interface IUserProfileRepository : IBaseRepository<UserProfile>
{
    bool EmailExists(string normalizedEmail);
    UserProfile? FindByEmail(string normalizedEmail);
}