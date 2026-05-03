using WebMoney.Data.Entities;

namespace WebMoney.Data.Repositories.Interfaces;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
    IReadOnlyList<Transaction> GetTransactionsByCardId(int cardId);
    IReadOnlyList<Transaction> GetTransactionsForPeriodByCard(int cardId, DateTime startDate, DateTime endDate);
}