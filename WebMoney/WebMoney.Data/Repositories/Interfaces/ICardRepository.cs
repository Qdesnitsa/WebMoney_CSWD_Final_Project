using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories.Interfaces;

public interface ICardRepository : IBaseRepository<Card>
{
    List<Card> GetCardsByUserEmail(string normalizedEmail);
    Card? GetCardWithUsersById(int cardId);
    void CreateDepositTransaction(int cardId, string normalizedUserEmail, decimal amount);
    bool CheckCardNumberAlreadyExists(string cardNumber);
}