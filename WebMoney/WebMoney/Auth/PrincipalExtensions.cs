using System.Security.Claims;

namespace WebMoney.Auth;

public static class PrincipalExtensions
{
    public static string? WebMoneyUserName(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Name);

    public static string? WebMoneyEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Email);
}
