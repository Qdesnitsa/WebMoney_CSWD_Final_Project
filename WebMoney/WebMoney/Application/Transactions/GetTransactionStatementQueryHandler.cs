using MediatR;
using WebMoney.Services;

namespace WebMoney.Application.Transactions;

public sealed class GetTransactionStatementQueryHandler(ITransactionService transactionService)
    : IRequestHandler<GetTransactionStatementQuery, TransactionStatementResult>
{
    public Task<TransactionStatementResult> Handle(
        GetTransactionStatementQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = transactionService.GetTransactionsByCardIdForPeriod(
            request.CardId,
            request.PeriodFrom,
            request.PeriodTo,
            request.PeriodKeysPresentInQuery);
        return Task.FromResult(result);
    }
}
