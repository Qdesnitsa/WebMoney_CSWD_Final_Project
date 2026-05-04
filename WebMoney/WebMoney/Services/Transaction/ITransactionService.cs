using WebMoney.Application.Transactions;
using WebMoney.Data.Entities;

namespace WebMoney.Services;

public interface ITransactionService
{
    TransactionStatementResult GetTransactionsByCardIdForPeriod(int cardId, int currentUserId, DateOnly? periodFrom, DateOnly? periodTo,
        bool periodKeysPresentInQuery);
    IReadOnlyList<Transaction> GetTransactionsByCardId(int cardId);
}