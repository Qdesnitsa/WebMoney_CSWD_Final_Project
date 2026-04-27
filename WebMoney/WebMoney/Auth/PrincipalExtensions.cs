using System.Security.Claims;

namespace WebMoney.Auth;

public static class PrincipalExtensions
{
    public static int? WebMoneyUserId(this ClaimsPrincipal user)
    {
        var rawUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(rawUserId, out var userId) ? userId : null;
    }

    public static string? WebMoneyUserName(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Name);
}
