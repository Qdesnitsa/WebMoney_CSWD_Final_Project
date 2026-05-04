using Microsoft.Extensions.Localization;
using WebMoney.Auth;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Application.Transactions;
using WebMoney.Data.Entities;

namespace WebMoney.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    ICardRepository cardRepository,
    IUserRepository userRepository,
    IStringLocalizer<SharedResource> localizer)
    : ITransactionService
{
    public TransactionStatementResult GetTransactionsByCardIdForPeriod(int cardId, int currentUserId, DateOnly? periodFrom,
        DateOnly? periodTo,
        bool periodKeysPresentInQuery)
    {
        var result = new TransactionStatementResult();

        var user = userRepository.GetById(currentUserId);
        var card = cardRepository.GetCardWithUsersAndCardLimitsById(cardId);
        if (card is null)
        {
            result.CardId = cardId;
            result.Errors.Add((string.Empty, localizer["Service_Err_CardNotFoundById", cardId].Value!));
            return result;
        }

        if (user is null || !CardPermissions.IsCardParticipant(user, card))
        {
            result.CardId = cardId;
            result.Errors.Add((string.Empty, localizer["Service_Err_CardMembersOnly"].Value!));
            return result;
        }

        result.CardId = cardId;
        result.CardNumber = card.Number;
        result.PeriodFrom = periodFrom;
        result.PeriodTo = periodTo;

        if (!periodFrom.HasValue || !periodTo.HasValue)
        {
            if (periodKeysPresentInQuery)
            {
                result.Errors.Add((string.Empty, localizer["Service_Err_BothDatesRequired"].Value!));
            }

            return result;
        }

        if (periodFrom > periodTo)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_PeriodFromAfterTo"].Value!));
            return result;
        }

        var rangeStart = periodFrom.Value.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        var rangeEnd = periodTo.Value.ToDateTime(TimeOnly.MaxValue).ToUniversalTime();
        result.Transactions = transactionRepository.GetTransactionsForPeriodByCard(cardId, rangeStart, rangeEnd);
        result.ShowEmptyPeriodMessage = result.Transactions.Count == 0;
        return result;
    }

    public IReadOnlyList<Transaction> GetTransactionsByCardId(int cardId) =>
        transactionRepository.GetTransactionsByCardId(cardId);
}