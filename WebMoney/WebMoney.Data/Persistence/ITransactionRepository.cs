using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Persistence;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
    IQueryable<Transaction> GetTransactionsByCardId(int cardId);
    public IQueryable<Transaction> GetTransactionsForPeriodByCard(int cardId, DateTime startDate, DateTime endDate);
    void Create(Transaction transaction);
}