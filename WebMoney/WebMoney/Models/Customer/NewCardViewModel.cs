using WebMoney.Data.Enum;
using WebMoney.Views;

namespace WebMoney.Models;

public class NewCardViewModel : BasePageViewModel
{
    public string CardNumber { get; set; } = string.Empty;
    public CurrencyCode CurrencyCode { get; set; }
    public decimal? DailyLimit { get; set; }
    public decimal? MonthlyLimit { get; set; }
    public decimal? PerOperationLimit { get; set; }
    public string PinCode { get; set; } = string.Empty;
    public DateOnly PeriodOfValidity { get; set; }
}
