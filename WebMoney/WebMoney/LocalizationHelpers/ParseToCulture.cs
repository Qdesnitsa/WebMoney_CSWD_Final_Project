using WebMoney.Enum;

namespace WebMoney.LocalizationHelpers;

public static class ParseToCulture
{
    public static bool TryParseCulture(string? value, out Locale culture)
    {
        culture = default;

        if (string.Equals(value, "ru-RU", StringComparison.OrdinalIgnoreCase))
        {
            culture = Locale.RuRu;
            return true;
        }

        if (string.Equals(value, "en-US", StringComparison.OrdinalIgnoreCase))
        {
            culture = Locale.EnUs;
            return true;
        }

        return false;
    }

    public static string ToCultureName(Locale culture) => culture switch
    {
        Locale.RuRu => "ru-RU",
        Locale.EnUs => "en-US",
        _ => throw new ArgumentOutOfRangeException(nameof(culture), culture, null)
    };
}
