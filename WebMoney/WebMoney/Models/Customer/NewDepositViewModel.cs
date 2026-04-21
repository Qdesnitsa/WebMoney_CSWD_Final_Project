using System.ComponentModel.DataAnnotations;
using WebMoney.Views;

namespace WebMoney.Models;

public class NewDepositViewModel : BasePageViewModel
{
    public int CardId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "Укажите сумму")]
    [Display(Name = "Сумма начисления (net)")]
    [Range(typeof(decimal), "0.01", "1000000000", ErrorMessage = "Сумма от 0,01 до 1 000 000 000")]
    public decimal Amount { get; set; }
}
