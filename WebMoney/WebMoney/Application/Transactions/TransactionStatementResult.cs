using WebMoney.Persistence.Entities;

namespace WebMoney.Application.Transactions;

public class TransactionStatementResult
{
    public int CardId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public DateOnly? PeriodFrom { get; set; }
    public DateOnly? PeriodTo { get; set; }
    public List<(string Field, string Message)> Errors { get; } = new();
    public bool Success => Errors.Count == 0;
    public List<Transaction> Transactions { get; set; } = new();
    public bool ShowEmptyPeriodMessage { get; set; }
}
