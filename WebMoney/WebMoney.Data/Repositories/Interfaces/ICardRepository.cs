using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories.Interfaces;

public interface ICardRepository : IBaseRepository<Card>
{
    List<Card> GetCardsByUserEmail(string normalizedEmail);
    Card? GetCardWithUsersById(int cardId);
}