using Microsoft.AspNetCore.Identity;

namespace WebMoney.Tools.HashPasswords;

internal sealed class HashUser;

internal sealed class HashCard;

public static class Program
{
    public static void Main(string[] args)
    {
        var userHasher = new PasswordHasher<HashUser>();
        var plainPassword = "Test123!";
        Console.WriteLine("Пароль пользователя (plain text): " + plainPassword);
        Console.WriteLine("users.hashed_password:");
        Console.WriteLine(userHasher.HashPassword(new HashUser(), plainPassword));
        Console.WriteLine();

        var cardHasher = new PasswordHasher<HashCard>();
        var plainPin = "1234";
        Console.WriteLine("PIN карты (plain text): " + plainPin);
        Console.WriteLine("cards.hashed_pin_code:");
        Console.WriteLine(cardHasher.HashPassword(new HashCard(), plainPin));
    }
}
