using WebMoney.Data.Persistence;
using WebMoney.Persistence.Entities;
using WebMoney.Persistence.Storage;

namespace WebMoney.Services;

public class TransactionService(ITransactionRepository transactionRepository, ICardRepository cardRepository)
    : ITransactionService
{
    public TransactionStatementResult GetStatement(int cardId, DateOnly? periodFrom, DateOnly? periodTo,
        bool periodKeysPresentInQuery)
    {
        var card = cardRepository.GetCardById(cardId);
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