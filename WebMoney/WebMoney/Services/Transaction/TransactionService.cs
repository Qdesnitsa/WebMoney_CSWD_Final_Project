using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public class TransactionService(ITransactionRepository transactionRepository, ICardRepository cardRepository)
    : ITransactionService
{
    public TransactionStatementResult GetTransactionsByCardIdForPeriod(int cardId, DateOnly? periodFrom, DateOnly? periodTo,
        bool periodKeysPresentInQuery)
    {
        var card = cardRepository.GetById(cardId);
        if (card is null)
        {
            return new TransactionStatementResult { IsCardMissing = true, CardId = cardId };
        }

        var partial = new TransactionStatementResult
        {
            CardId = cardId,
            CardNumber = card.Number,
            PeriodFrom = periodFrom,
            PeriodTo = periodTo
        };
        if (!periodFrom.HasValue || !periodTo.HasValue)
        {
            if (periodKeysPresentInQuery)
            {
                return new TransactionStatementResult
                {
                    CardId = partial.CardId,
                    CardNumber = partial.CardNumber,
                    PeriodFrom = partial.PeriodFrom,
                    PeriodTo = partial.PeriodTo,
                    ErrorMessage = "Укажите обе даты периода."
                };
            }

            return partial;
        }

        if (periodFrom > periodTo)
        {
            return new TransactionStatementResult
            {
                CardId = partial.CardId,
                CardNumber = partial.CardNumber,
                PeriodFrom = partial.PeriodFrom,
                PeriodTo = partial.PeriodTo,
                ErrorMessage = "Дата «с» не может быть позже даты «по»."
            };
        }

        var rangeStart = periodFrom.Value.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        var rangeEnd = periodTo.Value.ToDateTime(TimeOnly.MaxValue).ToUniversalTime();
        var rows = transactionRepository.GetTransactionsForPeriodByCard(cardId, rangeStart, rangeEnd);
        return new TransactionStatementResult
        {
            CardId = partial.CardId,
            CardNumber = partial.CardNumber,
            PeriodFrom = partial.PeriodFrom,
            PeriodTo = partial.PeriodTo,
            Transactions = rows
        };
    }

    public List<Transaction> GetTransactionsByCardId(int cardId) =>
        transactionRepository.GetTransactionsByCardId(cardId);
}