using WebMoney.Auth;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Application.Transactions;
using WebMoney.Data.Entities;

namespace WebMoney.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    ICardRepository cardRepository,
    IUserRepository userRepository)
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
            result.Errors.Add((string.Empty, $"Карта с id {cardId} не найдена"));
            return result;
        }

        if (user is null || !CardPermissions.IsCardParticipant(user, card))
        {
            result.CardId = cardId;
            result.Errors.Add((string.Empty, "Доступно только участникам карты"));
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
                result.Errors.Add((string.Empty, "Укажите обе даты периода."));
            }

            return result;
        }

        if (periodFrom > periodTo)
        {
            result.Errors.Add((string.Empty, "Дата «с» не может быть позже даты «по»."));
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