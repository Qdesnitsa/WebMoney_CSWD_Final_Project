using WebMoney.Persistence.Entities;

namespace WebMoney.Models;

public sealed class PrepareNewCardResult
{
    public bool Success => Errors.Count == 0;
    public List<(string Field, string Message)> Errors { get; } = new();
    public Card? Card { get; set; }
}