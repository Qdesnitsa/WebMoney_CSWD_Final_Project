using WebMoney.Data.Persistence;
using WebMoney.Persistence.Entities;

namespace WebMoney.Persistence.Storage;

public interface ICardRepository : IBaseRepository<Card>
{
    List<Card> GetCardsByUserEmail(string normalizedEmail);
}