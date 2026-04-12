using WebMoney.Data.Persistence;
using WebMoney.Persistence.Entities;

namespace WebMoney.Persistence.Storage;

public interface ICardRepository : IBaseRepository<Card>
{
    IQueryable<Card> GetAllCards();
    Card? GetCardById(int cardId);
    IQueryable<Card> GetCardsByUserEmail(string normalizedEmail);
    void Create(Card card);
}