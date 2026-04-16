namespace WebMoney.Persistence.Entities;

public class CardUserProfile
{
    public int CardId { get; set; }
    public Card Card { get; set; } = null!;
    public int UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public CardLimit? CardLimit { get; set; }
}