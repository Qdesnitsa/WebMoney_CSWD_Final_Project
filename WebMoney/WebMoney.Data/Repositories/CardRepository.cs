using Microsoft.EntityFrameworkCore;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Data.Enum;
using WebMoney.Data.Entities;

namespace WebMoney.Data.Repositories;

public class CardRepository(WebContext webContext)
    : BaseRepository<Card>(webContext), ICardRepository
{
    public IReadOnlyList<Card> GetCardsByUserId(int userId) =>
        webContext.Cards
            .Where(card => card.CardUserProfiles.Any(cup => cup.UserId == userId))
            .Include(card => card.CardUserProfiles.Where(cup => cup.UserId == userId))
            .ThenInclude(cup => cup.User)
            .ToList();

    public Card? GetCardWithUsersAndCardLimitsById(int cardId) =>
        webContext.Cards
            .Include(c => c.CardUserProfiles)
            .ThenInclude(cup => cup.User)
            .Include(c => c.CardUserProfiles)
            .ThenInclude(cup => cup.CardLimit)
            .FirstOrDefault(c => c.Id == cardId);

    public bool CheckCardNumberAlreadyExists(string cardNumber) => webContext.Cards.Any(c => c.Number == cardNumber);

    public void RemoveCardUserProfileWithOptionalLimit(Card card, CardUserProfile profile)
    {
        var limit = profile.CardLimit;
        card.CardUserProfiles.Remove(profile);
        if (limit is not null)
        {
            webContext.CardLimits.Remove(limit);
        }

        webContext.SaveChanges();
    }

    public void CreateDepositTransaction(int cardId, string normalizedUserEmail, decimal amount)
    {
        using var transaction = webContext.Database.BeginTransaction();

        webContext.Cards
            .Where(c => c.Id == cardId)
            .ExecuteUpdate(s => s
                .SetProperty(c => c.Balance, c => c.Balance + amount)
                .SetProperty(c => c.UpdatedAt, _ => DateTime.UtcNow)
                .SetProperty(c => c.UpdatedBy, _ => normalizedUserEmail));

        webContext.Transactions.Add(new Transaction
        {
            CardId = cardId,
            TransactionType = TransactionType.Deposit,
            TransactionStatus = TransactionStatus.Completed,
            CounterpartyId = 1,
            Amount = amount,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = normalizedUserEmail,
        });
        webContext.SaveChanges();
        transaction.Commit();
    }
}