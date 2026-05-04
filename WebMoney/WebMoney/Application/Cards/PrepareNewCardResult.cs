using WebMoney.Data.Entities;

namespace WebMoney.Application.Cards;

public sealed class PrepareNewCardResult
{
    public bool Success => Errors.Count == 0;
    public List<(string Field, string Message)> Errors { get; } = new();
    public Card? Card { get; set; }
}