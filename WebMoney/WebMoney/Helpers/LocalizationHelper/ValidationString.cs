using Microsoft.Extensions.Localization;

namespace WebMoney.LocalizationHelper;

public static class ValidationString
{
    public static string From(IStringLocalizer<SharedResource> localizer, string resourceName) =>
        localizer[resourceName].Value ?? resourceName;
}
