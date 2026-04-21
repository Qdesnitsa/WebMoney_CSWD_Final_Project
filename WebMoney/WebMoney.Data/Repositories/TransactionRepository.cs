using Microsoft.EntityFrameworkCore;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories;

public class TransactionRepository(WebContext webContext)
    : BaseRepository<Transaction>(webContext), ITransactionRepository
{
    public List<Transaction> GetTransactionsByCardId(int cardId) =>
        webContext.Transactions
            .AsNoTracking()
            .Include(t => t.Card)
            .Include(t => t.Counterparty)
            .Where(t => t.CardId == cardId)
            .OrderBy(t => t.CreatedAt)
            .ToList();

    public List<Transaction> GetTransactionsForPeriodByCard(int cardId, DateTime startDate, DateTime endDate) =>
        webContext.Transactions
            .AsNoTracking()
            .Include(t => t.Card)
            .Include(t => t.Counterparty)
            .Where(t => t.CardId == cardId)
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .OrderBy(t => t.CreatedAt)
            .ToList();
}