using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Persistence;

public class TransactionRepository(WebContext webContext)
    : BaseRepository<Transaction>(webContext), ITransactionRepository
{
    public List<Transaction> GetTransactionsByCardId(int cardId) =>
        webContext.Transactions.Where(c => c.Id == cardId).ToList();

    public List<Transaction> GetTransactionsForPeriodByCard(int cardId, DateTime startDate, DateTime endDate) =>
        webContext.Transactions
            .Where(c => c.Id == cardId)
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .OrderBy(t => t.CreatedAt).ToList();
}