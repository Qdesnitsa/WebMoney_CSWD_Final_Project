using WebMoney.Persistence.Entities;

namespace WebMoney.Persistence;

public interface IUserStore
{
    bool EmailExists(string normalizedEmail);

    void Create(User user);

    User FindByEmail(string normalizedEmail);
    List<User> GetAllUsers();
}