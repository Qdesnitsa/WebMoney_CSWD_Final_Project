using WebMoney.Data.Enum;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.ModelTransfer;

namespace WebMoney.Services;

public class DepositTransactionService(ICardRepository cardRepository, ILogger<DepositTransactionService> logger)
    : IDepositTransactionService
{
    public NewDepositPrepareResult SubmitNewDeposit(int cardId, string normalizedEmail, decimal amount)
    {
        var result = new NewDepositPrepareResult();
        var card = cardRepository.GetCardWithUsersById(cardId);
        if (card is null)
        {
            result.Errors.Add((string.Empty, "Карта не найдена"));
            return result;
        }

        if (card.CardStatus != CardStatus.Active)
        {
            result.Errors.Add((string.Empty, "Карта не активна"));
        }

        if (amount is < 0.01m or > 1_000_000_000m)
        {
            result.Errors.Add((nameof(amount), "Сумма вне допустимого диапазона"));
        }

        result.CardNumber = card.Number;

        if (!card.CardUserProfiles.Any(cup => cup.User.Email == normalizedEmail))
        {
            result.Errors.Add((string.Empty, "Нет доступа к этой карте"));
        }

        if (!result.Success)
        {
            return result;
        }
        
        cardRepository.CreateDepositTransaction(cardId, normalizedEmail, amount);
            
        return result;
    }
}