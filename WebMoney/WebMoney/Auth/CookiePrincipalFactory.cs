using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebMoney.Persistence.Entities;

namespace WebMoney.Auth;

public static class CookiePrincipalFactory
{
    public static ClaimsPrincipal CreateFor(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.Name, ClaimTypes.Role);
        return new ClaimsPrincipal(identity);
    }
}
