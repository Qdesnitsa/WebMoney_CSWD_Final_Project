namespace WebMoney.Application.Deposits;

public sealed record PrepareNewDepositCommand(
    int CardId,
    string NormalizedEmail,
    decimal Amount);
