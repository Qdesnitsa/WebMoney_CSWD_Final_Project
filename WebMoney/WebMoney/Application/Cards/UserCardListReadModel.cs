namespace WebMoney.Application.Cards;

public sealed class UserCardListReadModel
{
    public required int CardId { get; init; }
    public required string MaskedNumber { get; init; }
    public required string ValidThru { get; init; }
    public required string CreatedBy { get; init; }
    public required decimal Balance { get; init; }
    public required string CurrencyCode { get; init; }
    public required bool ShowUserManagement { get; init; }
    public required bool IsOwner { get; init; }
}
