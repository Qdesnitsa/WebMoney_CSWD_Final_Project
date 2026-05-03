using WebMoney.Auth;
using WebMoney.Data.Enum;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Application.Deposits;

namespace WebMoney.Services;

public class DepositTransactionService(
    ICardRepository cardRepository,
    IUserRepository userRepository,
    ILogger<DepositTransactionService> logger)
    : IDepositTransactionService
{
    public PrepareNewDepositResult SubmitNewDeposit(int cardId, int userId, decimal amount)
    {
        var result = new PrepareNewDepositResult();
        var card = cardRepository.GetCardWithUsersAndCardLimitsById(cardId);
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

        var user = userRepository.GetById(userId);
        if (user is null || !CardPermissions.IsCardParticipant(user, card))
        {
            result.Errors.Add((string.Empty, "Доступно только участникам карты"));
        }

        if (!result.Success)
        {
            return result;
        }

        var userEmail = card.CardUserProfiles!
            .First(cup => cup.UserId == userId)
            .User!.Email;

        cardRepository.CreateDepositTransaction(cardId, userEmail, amount);
            
        return result;
    }
}