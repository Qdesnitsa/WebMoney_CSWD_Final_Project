using WebMoney.Data.Persistence;
using WebMoney.Persistence.Entities;

namespace WebMoney.Persistence.Storage;

public interface ICardRepository : IBaseRepository<Card>
{
    List<Card> GetAllCards();
    Card? GetCardById(int cardId);
    List<Card> GetCardsByUserEmail(string normalizedEmail);
    void Create(Card card);
}