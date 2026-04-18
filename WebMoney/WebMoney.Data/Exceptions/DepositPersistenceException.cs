namespace WebMoney.Exceptions;

public class DepositPersistenceException : Exception
{
    public int CardId { get; }
    public decimal Amount { get; }

    public DepositPersistenceException(int cardId, decimal amount, Exception innerException)
        : base("Не удалось сохранить депозит в базе данных.", innerException)
    {
        CardId = cardId;
        Amount = amount;
    }
}