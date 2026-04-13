using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public sealed class AuthResult
{
    public bool Succeeded { get; private init; }
    public UserProfile? UserProfile { get; private init; }
    public string? ErrorMessage { get; private init; }

    private AuthResult(bool succeeded, UserProfile? userProfile, string? errorMessage)
    {
        Succeeded = succeeded;
        UserProfile = userProfile;
        ErrorMessage = errorMessage;
    }

    public static AuthResult Fail(string message) =>
        new(false, null, message);

    public static AuthResult Ok(UserProfile userProfile) =>
        new(true, userProfile, null);
}