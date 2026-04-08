using WebMoney.Persistence.Entities;

namespace WebMoney.Persistence.Storage;

public interface ICardStore
{
    List<Card> GetAllCards();
    public List<Transaction> GetTransactionsForPeriodByCard(int cardId, DateTime startDate, DateTime endDate);
    Card? GetCardById(int cardId);
}