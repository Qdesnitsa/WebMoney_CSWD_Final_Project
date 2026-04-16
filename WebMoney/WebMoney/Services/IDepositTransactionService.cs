using WebMoney.ModelTransfer;

namespace WebMoney.Services;

public interface IDepositTransactionService
{
    NewDepositPrepareResult SubmitNewDeposit(int cardId, string normalizedEmail, decimal amount);
}