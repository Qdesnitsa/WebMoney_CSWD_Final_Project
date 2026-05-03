using WebMoney.Data.Entities;

namespace WebMoney.Data.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    bool EmailExists(string normalizedEmail);
    User? FindByEmail(string normalizedEmail);
    void CreateWithProfile(UserProfile profile);
}