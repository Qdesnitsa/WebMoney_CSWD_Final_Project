using WebMoney.Views;

namespace WebMoney.Models;

public class EditUsersViewModel : BasePageViewModel
{
    public int CardId { get; set; }
    public string CardNumberMasked { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public bool CanManageUsers { get; set; }
    public bool CanManageCards { get; set; }
    public decimal? DailyLimit { get; set; }
    public decimal? MonthlyLimit { get; set; }
    public decimal? PerOperationLimit { get; set; }
}
