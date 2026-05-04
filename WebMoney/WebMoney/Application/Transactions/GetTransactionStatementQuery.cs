using MediatR;

namespace WebMoney.Application.Transactions;

public sealed record GetTransactionStatementQuery(
    int CardId,
    int CurrentUserId,
    DateOnly? PeriodFrom,
    DateOnly? PeriodTo,
    bool PeriodKeysPresentInQuery) : IRequest<TransactionStatementResult>;
