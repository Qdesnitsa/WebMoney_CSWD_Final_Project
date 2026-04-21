namespace WebMoney.Persistence.Entities;

public class CardLimit : BaseEntity
{
    public decimal? DailyLimit { get; set; }
    public decimal? MonthlyLimit { get; set; }
    public decimal? PerOperationLimit { get; set; }
    public virtual ICollection<CardUserProfile> CardUserProfiles { get; set; } = new List<CardUserProfile>();
}