namespace WebMoney.Persistence.Entities;

public class CardUserProfile
{
    public int CardId { get; set; }
    public virtual Card Card { get; set; } = null!;
    public int UserProfileId { get; set; }
    public virtual UserProfile UserProfile { get; set; } = null!;
    public int? CardLimitId { get; set; }
    public virtual CardLimit? CardLimit { get; set; }
}