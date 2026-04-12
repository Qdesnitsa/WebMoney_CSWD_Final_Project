using WebMoney.Data.Persistence;
using WebMoney.Persistence.Entities;

namespace WebMoney.Persistence;

public interface IUserRepository : IBaseRepository<UserProfile>
{
    bool EmailExists(string normalizedEmail);

    void Create(UserProfile user);

    UserProfile FindByEmail(string normalizedEmail);
    IQueryable<UserProfile> GetAllUsers();
}