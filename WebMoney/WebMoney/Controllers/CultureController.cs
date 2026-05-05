using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using WebMoney.LocalizationHelpers;

namespace WebMoney.Controllers;

[AllowAnonymous]
public class CultureController : Controller
{
    [HttpGet]
    public IActionResult Set(string culture, string? returnUrl = null)
    {
        if (!ParseToCulture.TryParseCulture(culture, out var appCulture))
        {
            return BadRequest();
        }

        var cultureInfo = CultureInfo.GetCultureInfo(ParseToCulture.ToCultureName(appCulture));
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
