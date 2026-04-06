using WebMoney.Enum;
using WebMoney.Persistence.Entities;
using Type = WebMoney.Enum.TxnType;

namespace WebMoney.Persistence.Storage;

public class CardStoreInMemory : ICardStore
{
    private List<Card> _cards = new()
    {
        new()
        {
            Id = 1,
            Url = "/images/card_wrappers/card2.jpg",
            Number = "1212121212121212",
            Transactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = 1,
                    DateTime = new DateTime(2026, 4, 2, 10, 30, 55, DateTimeKind.Utc),
                    TxnType = Type.Deposit,
                    Status = Status.Initialized,
                    CurrencyCode = CurrencyCode.BYN,
                    RRN = "1234567899",
                    Counterparty = "Test",
                    Amount = 123m
                },
                new Transaction
                {
                    Id = 2,
                    DateTime = new DateTime(2026, 4, 1, 10, 30, 55, DateTimeKind.Utc),
                    TxnType = Type.Withdrawal,
                    Status = Status.Completed,
                    CurrencyCode = CurrencyCode.BYN,
                    RRN = "1234567898",
                    Counterparty = "Test1",
                    Amount = 99.45m
                },
                new Transaction
                {
                    Id = 3,
                    DateTime = new DateTime(2026, 3, 31, 10, 30, 55, DateTimeKind.Utc),
                    TxnType = Type.Withdrawal,
                    Status = Status.Completed,
                    CurrencyCode = CurrencyCode.BYN,
                    RRN = "1234567897",
                    Counterparty = "Test1",
                    Amount = 99.44m
                }
            },
        },
        new()
        {
            Url = "/images/card_wrappers/card1.jpg",
            Number = "2323232323232323",
            Transactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = 4,
                    DateTime = new DateTime(2026, 4, 2, 10, 30, 55, DateTimeKind.Utc),
                    TxnType = Type.Deposit,
                    Status = Status.Initialized,
                    CurrencyCode = CurrencyCode.BYN,
                    RRN = "1234567896",
                    Counterparty = "Test",
                    Amount = 0.45m
                },
                new Transaction
                {
                    Id = 5,
                    DateTime = new DateTime(2026, 4, 1, 10, 30, 55, DateTimeKind.Utc),
                    TxnType = Type.Withdrawal,
                    Status = Status.Completed,
                    CurrencyCode = CurrencyCode.BYN,
                    RRN = "1234567895",
                    Counterparty = "Test1",
                    Amount = 9.99m
                },
                new Transaction
                {
                    Id = 6,
                    DateTime = new DateTime(2026, 3, 31, 10, 30, 55, DateTimeKind.Utc),
                    TxnType = Type.Withdrawal,
                    Status = Status.Completed,
                    CurrencyCode = CurrencyCode.BYN,
                    RRN = "1234567894",
                    Counterparty = "Test1",
                    Amount = 5.45m
                }
            },
        },
        new()
        {
            Id = 3,
            Url = "/images/card_wrappers/card3.jpg",
            Number = "3434343434343434",
            Transactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = 7,
                    DateTime = new DateTime(2026, 4, 2, 10, 30, 55, DateTimeKind.Utc),
                    TxnType = Type.Deposit,
                    Status = Status.Initialized,
                    CurrencyCode = CurrencyCode.BYN,
                    RRN = "1234567893",
                    Counterparty = "Test",
                    Amount = 12399.45m
                },
                new Transaction
                {
                    Id = 8,
                    DateTime = new DateTime(2026, 4, 1, 10, 30, 55, DateTimeKind.Utc),
                    TxnType = Type.Withdrawal,
                    Status = Status.Completed,
                    CurrencyCode = CurrencyCode.BYN,
                    RRN = "1234567892",
                    Counterparty = "Test1",
                    Amount = 99m
                },
                new Transaction
                {
                    Id = 9,
                    DateTime = new DateTime(2026, 3, 31, 10, 30, 55, DateTimeKind.Utc),
                    TxnType = Type.Withdrawal,
                    Status = Status.Completed,
                    CurrencyCode = CurrencyCode.BYN,
                    RRN = "1234567891",
                    Counterparty = "Test1",
                    Amount = 99.99m
                }
            },
        }
    };

    public List<Card> GetAllCards() => _cards;

    public List<Transaction> GetTransactionsForPeriodByCard(int cardId, DateTime startDate, DateTime endDate) =>
        GetAllCards()
            .Where(c => c.Id == cardId)
            .SelectMany(c => c.Transactions ?? Enumerable.Empty<Transaction>())
            .Where(t => t.DateTime >= startDate && t.DateTime <= endDate)
            .OrderBy(t => t.DateTime)
            .ToList();
}