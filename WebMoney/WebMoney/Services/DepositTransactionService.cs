using Microsoft.EntityFrameworkCore;
using WebMoney.Data;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Enum;
using WebMoney.ModelTransfer;
using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public class DepositTransactionService(ICardRepository cardRepository, WebContext webContext)
    : IDepositTransactionService
{
    public NewDepositPrepareResult SubmitNewDeposit(int cardId, string normalizedEmail, decimal amount)
    {
        var result = new NewDepositPrepareResult();
        if (amount is < 0.01m or > 1_000_000_000m)
        {
            result.Errors.Add((nameof(amount), "Сумма вне допустимого диапазона"));
        }

        var card = cardRepository.GetCardWithUsersById(cardId);
        if (card is null)
        {
            result.Errors.Add((string.Empty, "Карта не найдена"));
            return result;
        }

        result.CardNumber = card.Number;

        if (!card.CardUserProfiles.Any(cup => cup.UserProfile.User.Email == normalizedEmail))
        {
            result.Errors.Add((string.Empty, "Нет доступа к этой карте"));
        }

        if (!result.Success)
        {
            return result;
        }
        
        Deposit(cardId, normalizedEmail, amount);
        
        return result;
    }

    private void Deposit(int cardId, string normalizedUserEmail, decimal amount)
    {
        using var txn = webContext.Database.BeginTransaction();
        try
        {
            var affected = webContext.Cards
                .Where(c => c.Id == cardId)
                .ExecuteUpdate(s => s
                    .SetProperty(c => c.Balance, c => c.Balance + amount)
                    .SetProperty(c => c.UpdatedAt, _ => DateTime.UtcNow)
                    .SetProperty(c => c.UpdatedBy, _ => normalizedUserEmail));
            if (affected == 0)
                throw new InvalidOperationException("Карта не найдена");

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
            txn.Commit();
        }
        catch
        {
            txn.Rollback();
            throw;
        }
    }
}