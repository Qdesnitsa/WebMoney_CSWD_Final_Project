using Microsoft.AspNetCore.Identity;
using WebMoney.Enum;
using WebMoney.Persistence.Entities;

namespace WebMoney.Persistence;

public class UserStoreInMemory(IPasswordHasher<User> passwordHasher) : IUserStore
{
    private List<User> _users = new()
    {
        new()
        {
            Id = 1, 
            UserName = "test1", 
            Email = "test1@example.com",
            Role = Role.User,
            HashedPassword = passwordHasher.HashPassword(new(), "Test12345!")
        },
        new()
        {
            Id = 2, 
            UserName = "test2",
            Email = "test2@example.com",
            Role = Role.User,
            HashedPassword = passwordHasher.HashPassword(new(), "Test12346!")
        },
        new()
        {
            Id = 3, 
            UserName = "test3", 
            Email = "test3@example.com",
            Role = Role.Admin,
            HashedPassword = passwordHasher.HashPassword(new(), "Test12347!")
        }
    };

    public bool EmailExists(string normalizedEmail) =>
        _users.Exists(u => u.Email == normalizedEmail);

    public void Create(User user) => _users.Add(user);

    public User FindByEmail(string normalizedEmail) =>
        _users.FirstOrDefault(u => u.Email == normalizedEmail);

    public List<User> GetAllUsers() => _users;
}