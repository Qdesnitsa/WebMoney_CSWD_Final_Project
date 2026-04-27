using WebMoney.Application.Deposits;

namespace WebMoney.Services;

public interface IDepositTransactionService
{
    PrepareNewDepositResult SubmitNewDeposit(int cardId, int userId, decimal amount);
}