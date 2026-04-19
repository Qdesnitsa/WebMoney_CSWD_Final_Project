using Microsoft.EntityFrameworkCore;
using WebMoney.Data;
using WebMoney.Data.Enum;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Exceptions;
using WebMoney.ModelTransfer;
using WebMoney.Persistence.Entities;

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

        if (!card.CardUserProfiles.Any(cup => cup.UserProfile.User.Email == normalizedEmail))
        {
            result.Errors.Add((string.Empty, "Нет доступа к этой карте"));
        }

        if (!result.Success)
        {
            return result;
        }

        try
        {
            cardRepository.CreateDepositTransaction(cardId, normalizedEmail, amount);
        }
        catch (CardNotFoundException ex)
        {
            logger.LogWarning(ex, "Карта не найдена. CardId={CardId}", ex.CardId);
            result.Errors.Add((string.Empty, "Карта не найдена. Проверьте выбранную карту."));
            return result;
        }
        catch (DepositPersistenceException ex)
        {
            logger.LogError(ex, "Ошибка записи пополнения карты в БД. CardId={CardId}, Amount={Amount}", ex.CardId, ex.Amount);
            result.Errors.Add((string.Empty, "Не удалось сохранить операцию. Попробуйте позже."));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Пополнение карты не выполнено. CardId={CardId}", cardId);
            result.Errors.Add((string.Empty, "Не удалось выполнить операцию. Попробуйте позже."));
            return result;
        }

        return result;
    }
}