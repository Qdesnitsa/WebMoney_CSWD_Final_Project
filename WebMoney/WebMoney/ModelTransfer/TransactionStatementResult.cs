using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public sealed class TransactionStatementResult
{
    public int CardId { get; init; }
    public bool IsCardMissing { get; init; }
    public string CardNumber { get; init; } = "";
    public DateOnly? PeriodFrom { get; init; }
    public DateOnly? PeriodTo { get; init; }
    public string? ErrorMessage { get; init; }
    public IReadOnlyList<Transaction> Transactions { get; init; } = Array.Empty<Transaction>();
}