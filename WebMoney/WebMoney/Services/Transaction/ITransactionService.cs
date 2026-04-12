using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public interface ITransactionService
{
    TransactionStatementResult GetStatement(int cardId, DateOnly? periodFrom, DateOnly? periodTo,
        bool periodKeysPresentInQuery);
    IQueryable<Transaction> GetTransactionsByCardId(int cardId);
}