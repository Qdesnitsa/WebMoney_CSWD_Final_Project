namespace WebMoney.Exceptions;

public class CardNotFoundException : Exception
{
    public int CardId { get; }
    public CardNotFoundException(int cardId) : base($"Карта не найдена: {cardId}") => CardId = cardId;
}
