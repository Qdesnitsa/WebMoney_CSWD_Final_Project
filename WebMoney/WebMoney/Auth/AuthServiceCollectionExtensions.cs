using Microsoft.AspNetCore.Authentication.Cookies;
using WebMoney.Data.Enum;

namespace WebMoney.Auth;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddWebMoneyCookieAuth(this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.LoginPath = "/SignIn/SignIn";
                options.LogoutPath = "/SignOut/LogOut";
                options.AccessDeniedPath = "/Home/Forbidden";
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.SignedIn, p => p.RequireAuthenticatedUser())
            .AddPolicy(AuthPolicies.UserOnly, p => p.RequireRole(nameof(Role.User)))
            .AddPolicy(AuthPolicies.AdminOnly, p => p.RequireRole(nameof(Role.Admin)));

        return services;
    }
}