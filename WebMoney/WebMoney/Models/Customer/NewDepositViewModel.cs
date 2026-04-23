using WebMoney.Views;

namespace WebMoney.Models;

public class NewDepositViewModel : BasePageViewModel
{
    public int CardId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
