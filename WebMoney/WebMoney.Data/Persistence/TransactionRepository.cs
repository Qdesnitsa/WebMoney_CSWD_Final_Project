using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Persistence;

public class TransactionRepository(WebContext webContext) : ITransactionRepository
{
    public IQueryable<Transaction> GetTransactionsByCardId(int cardId) =>
        webContext.Transactions.Where(c => c.Id == cardId);

    public IQueryable<Transaction> GetTransactionsForPeriodByCard(int cardId, DateTime startDate, DateTime endDate) =>
        webContext.Transactions
            .Where(c => c.Id == cardId)
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .OrderBy(t => t.CreatedAt);

    public void Create(Transaction transaction)
    {
        throw new NotImplementedException();
    }
}