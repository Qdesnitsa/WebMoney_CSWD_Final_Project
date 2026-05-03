using WebMoney.Data.Entities;

namespace WebMoney.Data.Repositories.Interfaces;

public interface ICardRepository : IBaseRepository<Card>
{
    IReadOnlyList<Card> GetCardsByUserId(int userId);
    Card? GetCardWithUsersAndCardLimitsById(int cardId);
    void CreateDepositTransaction(int cardId, string normalizedUserEmail, decimal amount);
    bool CheckCardNumberAlreadyExists(string cardNumber);
    void RemoveCardUserProfileWithOptionalLimit(Card card, CardUserProfile profile);
}