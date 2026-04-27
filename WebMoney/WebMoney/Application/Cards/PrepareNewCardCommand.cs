using MediatR;
using WebMoney.Data.Enum;

namespace WebMoney.Application.Cards;

public sealed record PrepareNewCardCommand(
    int UserId,
    string CardNumber,
    CurrencyCode CurrencyCode,
    decimal? DailyLimit,
    decimal? MonthlyLimit,
    decimal? PerOperationLimit,
    string PinCode) : IRequest<PrepareNewCardResult>;
