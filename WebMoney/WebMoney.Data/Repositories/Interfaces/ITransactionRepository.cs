using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Persistence;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
    List<Transaction> GetTransactionsByCardId(int cardId);
    List<Transaction> GetTransactionsForPeriodByCard(int cardId, DateTime startDate, DateTime endDate);
}