namespace WebMoney.Models;

public class CardViewModel
{
    public int Id { get; set; }
    public string Number { get; set; }
    public string UserName { get; set; }
    public List<CardViewModel> Cards { get; set; }
    public string Url { get; set; } = string.Empty;
}