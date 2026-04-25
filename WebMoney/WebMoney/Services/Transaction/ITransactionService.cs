using WebMoney.Application.Transactions;
using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public interface ITransactionService
{
    TransactionStatementResult GetTransactionsByCardIdForPeriod(int cardId, DateOnly? periodFrom, DateOnly? periodTo,
        bool periodKeysPresentInQuery);
    List<Transaction> GetTransactionsByCardId(int cardId);
}