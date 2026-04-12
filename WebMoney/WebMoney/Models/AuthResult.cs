using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public class AuthResult
{
    public bool Succeeded { get; private init; }
    public User? User { get; private init; }
    public string? ErrorMessage { get; private init; }

    private AuthResult(bool succeeded, User? user, string? errorMessage)
    {
        Succeeded = succeeded;
        User = user;
        ErrorMessage = errorMessage;
    }

    public static AuthResult Fail(string message) =>
        new(false, null, message);

    public static AuthResult Ok(User user) =>
        new(true, user, null);
}