using MediatR;

namespace WebMoney.Application.Deposits;

public sealed record PrepareNewDepositCommand(
    int CardId,
    int UserId,
    decimal Amount) : IRequest<PrepareNewDepositResult>;
