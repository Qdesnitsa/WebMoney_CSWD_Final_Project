using WebMoney.Persistence.Entities;

namespace WebMoney.ModelTransfer;

/// <summary>
/// Результат выборки выписки по карте и периоду (по смыслу близко к <see cref="NewDepositPrepareResult"/>).
/// </summary>
public class TransactionStatementResult
{
    public int CardId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public DateOnly? PeriodFrom { get; set; }
    public DateOnly? PeriodTo { get; set; }

    public List<(string Field, string Message)> Errors { get; } = new();

    public bool Success => Errors.Count == 0;

    public List<Transaction> Transactions { get; set; } = new();

    /// <summary>
    /// Период задан корректно, запрос выполнен, операций за интервал нет (показ «пустой период» в UI).
    /// </summary>
    public bool ShowEmptyPeriodMessage { get; set; }
}
