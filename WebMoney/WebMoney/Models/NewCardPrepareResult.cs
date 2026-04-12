namespace WebMoney.Models;

public class NewCardPrepareResult
{
    public bool Success => Errors.Count == 0;
    public List<(string Field, string Message)> Errors { get; } = new();
}