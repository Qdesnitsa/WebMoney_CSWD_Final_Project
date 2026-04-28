using WebMoney.Views;

namespace WebMoney.Models;

public class AddCardUserViewModel : BasePageViewModel
{
    public int CardId { get; set; }
    public string CardNumberMasked { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool CanManageUsers { get; set; }
    public bool CanManageCards { get; set; }
    public decimal? DailyLimit { get; set; }
    public decimal? MonthlyLimit { get; set; }
    public decimal? PerOperationLimit { get; set; }
}
