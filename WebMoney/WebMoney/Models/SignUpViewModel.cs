using System.ComponentModel.DataAnnotations;
using WebMoney.Views;

namespace WebMoney.Models;

public class SignUpViewModel : BasePageViewModel
{
    [Required(ErrorMessage = "Укажите username")]
    [StringLength(256, MinimumLength = 3, ErrorMessage = "Username: 3–256 символов")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$",
        ErrorMessage = "Только латинские буквы, цифры, _ и -")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите email")]
    [EmailAddress(ErrorMessage = "Введите корректный адрес email")]
    [StringLength(256, MinimumLength = 5, ErrorMessage = "Email: 5–256 символов")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите пароль")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль: минимум 8 символов")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Пароль: латиница, цифра, заглавная и строчная буквы, от 8 символов")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите пароль")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; } = string.Empty;
}