using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace WebMoney.Controllers;

[AllowAnonymous]
public class CultureController : Controller
{
    private static readonly HashSet<string> SupportedCultures =
        new(StringComparer.OrdinalIgnoreCase) { "ru-RU", "en-US" };

    [HttpGet]
    public IActionResult Set(string culture, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(culture) || !SupportedCultures.Contains(culture))
        {
            return BadRequest();
        }

        var cultureInfo = CultureInfo.GetCultureInfo(culture);
        var safeReturn = string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl)
            ? "~/"
            : returnUrl;

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureInfo)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });

        return LocalRedirect(safeReturn);
    }
}
