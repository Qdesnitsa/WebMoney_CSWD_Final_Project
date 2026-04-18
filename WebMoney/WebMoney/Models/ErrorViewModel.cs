using WebMoney.Views;

namespace WebMoney.Models;

public class ErrorViewModel : BasePageViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}