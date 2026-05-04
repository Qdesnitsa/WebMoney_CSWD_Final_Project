using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using WebMoney.Application.Auth;
using WebMoney.Data.Enum;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Data.Entities;

namespace WebMoney.Services;

public class AuthService(
    IPasswordHasher<User> passwordHasher,
    IUserRepository userRepository,
    IStringLocalizer<SharedResource> localizer)
    : IAuthService
{
    public AuthResult TrySignIn(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = userRepository.FindByEmail(normalizedEmail);
        if (user is null)
        {
            return AuthResult.Fail(localizer["Service_Err_InvalidCredentials"].Value!);
        }

        var verifyPassword =
            passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);
        if (verifyPassword != PasswordVerificationResult.Success &&
            verifyPassword != PasswordVerificationResult.SuccessRehashNeeded)
        {
            return AuthResult.Fail(localizer["Service_Err_InvalidCredentials"].Value!);
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
            return AuthResult.Fail(localizer["Service_Err_EmailAlreadyRegistered"].Value!);
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