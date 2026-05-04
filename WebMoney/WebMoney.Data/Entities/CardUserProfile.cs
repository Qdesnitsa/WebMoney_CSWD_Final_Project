namespace WebMoney.Data.Entities;

public class CardUserProfile : BaseEntity
{
    public int UserId { get; set; }
    public int CardId { get; set; }
    public int? CardLimitId { get; set; }
    public bool CanManageUsers { get; set; }
    public virtual User User { get; set; }
    public virtual Card Card { get; set; }
    public virtual CardLimit? CardLimit { get; set; }
}