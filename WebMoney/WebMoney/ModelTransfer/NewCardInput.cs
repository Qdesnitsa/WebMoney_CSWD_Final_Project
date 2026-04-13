using WebMoney.Enum;

namespace WebMoney.ModelTransfer;

public class NewCardInput
{
    public string CardNumber { get; init; } = "";
    public CurrencyCode CurrencyCode { get; init; }
    public decimal? DailyLimit { get; init; }
    public decimal? MonthlyLimit { get; init; }
    public decimal? PerOperationLimit { get; init; }
    public string PinCode { get; init; } = "";
}