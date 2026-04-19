using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Data.Enum;
using WebMoney.Exceptions;
using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories;

public class CardRepository(WebContext webContext)
    : BaseRepository<Card>(webContext), ICardRepository
{
    public List<Card> GetCardsByUserEmail(string normalizedEmail) =>
        webContext.Cards
            .Where(card => card.CardUserProfiles
                .Any(cup => cup.UserProfile.User.Email == normalizedEmail))
            .ToList();

    public Card? GetCardWithUsersById(int cardId) =>
        webContext.Cards
            .Include(c => c.CardUserProfiles)
            .ThenInclude(cup => cup.UserProfile)
            .ThenInclude(up => up.User)
            .FirstOrDefault(c => c.Id == cardId);

    public void CreateDepositTransaction(int cardId, string normalizedUserEmail, decimal amount)
    {
        using var txn = webContext.Database.BeginTransaction();
        try
        {
            var rowsAffected = webContext.Cards
                .Where(c => c.Id == cardId)
                .ExecuteUpdate(s => s
                    .SetProperty(c => c.Balance, c => c.Balance + amount)
                    .SetProperty(c => c.UpdatedAt, _ => DateTime.UtcNow)
                    .SetProperty(c => c.UpdatedBy, _ => normalizedUserEmail));
            if (rowsAffected == 0)
            {
                throw new CardNotFoundException(cardId);
            }

            webContext.Transactions.Add(new Transaction
            {
                CardId = cardId,
                TransactionType = TransactionType.Deposit,
                TransactionStatus = TransactionStatus.Completed,
                CounterpartyId = 3,
                Amount = amount,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = normalizedUserEmail,
            });
            webContext.SaveChanges();
            txn.Commit();
        }
        catch (DbUpdateException ex)
        {
            throw new DepositPersistenceException(cardId, amount, ex);
        }
    }
}