namespace WebMoney.Models;

public static class CardNumberMask
{
    public static string Mask(string? number)
    {
        var digits = new string((number ?? string.Empty).Where(char.IsDigit).ToArray());
        if (digits.Length < 4)
        {
            return "••••";
        }

        var last4 = digits[^4..];
        return $"**** **** **** {last4}";
    }
}
