using WebMoney.Application.Deposits;

namespace WebMoney.Services;

public interface IDepositTransactionService
{
    PrepareNewDepositResult SubmitNewDeposit(int cardId, string normalizedEmail, decimal amount);
}