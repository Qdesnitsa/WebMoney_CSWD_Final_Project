using Microsoft.AspNetCore.Identity;
using WebMoney.Enum;
using WebMoney.Persistence;
using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public class AuthService(IPasswordHasher<User> passwordHasher, IUserStore userStore) : IAuthService
{
    public AuthResult TrySignIn(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = userStore.FindByEmail(normalizedEmail);
        if (user is null)
        {
            return AuthResult.Fail("Неверный email или пароль");
        }

        var verifyPassword = passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);
        if (verifyPassword != PasswordVerificationResult.Success &&
            verifyPassword != PasswordVerificationResult.SuccessRehashNeeded)
        {
            return AuthResult.Fail("Неверный email или пароль");
        }

        if (verifyPassword == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.HashedPassword = passwordHasher.HashPassword(user, password);
        }

        return AuthResult.Ok(user);
    }

    public AuthResult Register(string username, string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (userStore.EmailExists(normalizedEmail))
        {
            return AuthResult.Fail("Пользователь с таким email уже зарегистрирован");
        }

        var user = new User
        {
            Email = normalizedEmail,
            UserName = username,
            Role = Role.User
        };

        user.HashedPassword = passwordHasher.HashPassword(user, password);
        userStore.Create(user);

        return AuthResult.Ok(user);
    }
}