namespace WebMoney.Models;

public class CardViewModel
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string ValidThru { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public List<CardViewModel> Cards { get; set; } = [];
}