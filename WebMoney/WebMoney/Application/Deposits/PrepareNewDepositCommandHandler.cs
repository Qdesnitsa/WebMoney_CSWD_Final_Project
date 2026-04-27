using MediatR;
using WebMoney.Services;

namespace WebMoney.Application.Deposits;

public sealed class PrepareNewDepositCommandHandler(IDepositTransactionService depositTransactionService)
    : IRequestHandler<PrepareNewDepositCommand, PrepareNewDepositResult>
{
    public Task<PrepareNewDepositResult> Handle(PrepareNewDepositCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = depositTransactionService.SubmitNewDeposit(
            request.CardId,
            request.UserId,
            request.Amount);
        return Task.FromResult(result);
    }
}
