using System.ComponentModel.DataAnnotations;
using WebMoney.Views;

namespace WebMoney.Models;

public class SignInViewModel : BasePageViewModel
{
    public string Email { get; set; } = string.Empty;
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}