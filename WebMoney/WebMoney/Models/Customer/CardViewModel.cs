using WebMoney.Views;

namespace WebMoney.Models;

public class CardViewModel : BasePageViewModel
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string ValidThru { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public bool ShowPaymentOperations { get; set; }
    public bool ShowUserManagement { get; set; }
    public bool IsOwner { get; set; }

    public List<CardViewModel> Cards { get; set; } = [];
}