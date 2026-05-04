using Microsoft.Extensions.Localization;
using WebMoney;

namespace WebMoney.Localization;

public static class ValidationString
{
    public static string From(IStringLocalizer<SharedResource> localizer, string resourceName) =>
        localizer[resourceName].Value ?? resourceName;
}
