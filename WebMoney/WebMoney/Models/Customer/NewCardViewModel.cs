using System.ComponentModel.DataAnnotations;
using WebMoney.Data.Enum;
using WebMoney.Views;

namespace WebMoney.Models;

public class NewCardViewModel : BasePageViewModel
{
    [RegularExpression(@"^[1-9]\d{15}$", ErrorMessage = "Номер карты — 16 цифр, не начинается с 0")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите валюту")]
    public CurrencyCode CurrencyCode { get; set; }
    public decimal? DailyLimit { get; set; }

    public decimal? MonthlyLimit { get; set; }

    public decimal? PerOperationLimit { get; set; }

    [Required(ErrorMessage = "Введите PIN-код")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN-код — ровно 4 цифры")]
    public string PinCode { get; set; } = string.Empty;

    [Required]
    public DateOnly PeriodOfValidity { get; set; }
}
