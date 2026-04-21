using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories.Interfaces;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
    List<Transaction> GetTransactionsByCardId(int cardId);
    List<Transaction> GetTransactionsForPeriodByCard(int cardId, DateTime startDate, DateTime endDate);
}