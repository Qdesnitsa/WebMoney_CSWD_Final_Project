using Microsoft.AspNetCore.Identity;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Data.Enum;
using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public class AuthService(IPasswordHasher<User> passwordHasher, IUserRepository userRepository)
    : IAuthService
{
    public AuthResult TrySignIn(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = userRepository.FindByEmail(normalizedEmail);
        if (user is null)
        {
            return AuthResult.Fail("Неверный email или пароль");
        }

        var verifyPassword =
            passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);
        if (verifyPassword != PasswordVerificationResult.Success &&
            verifyPassword != PasswordVerificationResult.SuccessRehashNeeded)
        {
            return AuthResult.Fail("Неверный email или пароль");
        }

        if (verifyPassword == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.HashedPassword = passwordHasher.HashPassword(user, password);
            userRepository.SaveChanges();
        }

        return AuthResult.Ok(user);
    }

    public AuthResult Register(string username, string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (userRepository.EmailExists(normalizedEmail))
        {
            return AuthResult.Fail("Пользователь с таким email уже зарегистрирован");
        }

        var userProfile = new UserProfile
        {
            User = new User
            {
                Email = normalizedEmail,
                UserName = username,
                Role = Role.User,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = email
            },
            CreatedAt = DateTime.UtcNow,
            CreatedBy = normalizedEmail
        };

        userProfile.User.HashedPassword = passwordHasher.HashPassword(userProfile.User, password);
        userRepository.CreateWithProfile(userProfile);

        return AuthResult.Ok(userProfile.User);
    }
}