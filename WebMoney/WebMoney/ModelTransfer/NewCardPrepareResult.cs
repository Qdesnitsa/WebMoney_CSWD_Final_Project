namespace WebMoney.Models;

public sealed class NewCardPrepareResult
{
    public bool Success => Errors.Count == 0;
    public List<(string Field, string Message)> Errors { get; } = new();
}