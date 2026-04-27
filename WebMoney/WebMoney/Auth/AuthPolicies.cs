namespace WebMoney.Auth;

public static class AuthPolicies
{
    public const string SignedIn = nameof(SignedIn);
    public const string UserOnly = nameof(UserOnly);
    public const string AdminOnly = nameof(AdminOnly);
}
