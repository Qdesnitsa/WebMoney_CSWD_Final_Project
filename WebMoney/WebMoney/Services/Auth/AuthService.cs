using Microsoft.AspNetCore.Identity;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Enum;
using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public class AuthService(IPasswordHasher<User> passwordHasher, IUserProfileRepository userProfileRepository)
    : IAuthService
{
    public AuthResult TrySignIn(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var userProfile = userProfileRepository.FindByEmail(normalizedEmail);
        if (userProfile is null)
        {
            return AuthResult.Fail("Неверный email или пароль");
        }

        var verifyPassword =
            passwordHasher.VerifyHashedPassword(userProfile.User, userProfile.User.HashedPassword, password);
        if (verifyPassword != PasswordVerificationResult.Success &&
            verifyPassword != PasswordVerificationResult.SuccessRehashNeeded)
        {
            return AuthResult.Fail("Неверный email или пароль");
        }

        if (verifyPassword == PasswordVerificationResult.SuccessRehashNeeded)
        {
            userProfile.User.HashedPassword = passwordHasher.HashPassword(userProfile.User, password);
        }

        return AuthResult.Ok(userProfile);
    }

    public AuthResult Register(string username, string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (userProfileRepository.EmailExists(normalizedEmail))
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
            CardUserProfiles = new HashSet<CardUserProfile>(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = normalizedEmail
        };

        userProfile.User.HashedPassword = passwordHasher.HashPassword(userProfile.User, password);
        userProfileRepository.Create(userProfile);

        return AuthResult.Ok(userProfile);
    }
}