using System.ComponentModel.DataAnnotations;
using WebMoney.Views;

namespace WebMoney.Models;

public class SignUpViewModel : BasePageViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [DataType(DataType.Password)] 
    public string Password { get; set; } = string.Empty;
    [DataType(DataType.Password)] 
    public string ConfirmPassword { get; set; } = string.Empty;
}