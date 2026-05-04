using WebMoney.Views;

namespace WebMoney.Models.EditCardUsers;

public class EditUsersViewModel : BasePageViewModel
{
    public int CardId { get; set; }
    public string CardNumberMasked { get; set; } = string.Empty;
    public bool CurrentUserMayManageUsers { get; set; }

    public List<CardUserAccessViewModel> Users { get; set; } = [];

    public class CardUserAccessViewModel
    {
        public int CardUserProfileId { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public bool IsOwner { get; set; }
        public bool CanManageUsers { get; set; }
        public decimal? DailyLimit { get; set; }
        public decimal? MonthlyLimit { get; set; }
        public decimal? PerOperationLimit { get; set; }
    }

    public class UpdateCardUserAccessViewModel
    {
        public int CardId { get; set; }
        public int CardUserProfileId { get; set; }
        public decimal? DailyLimit { get; set; }
        public decimal? MonthlyLimit { get; set; }
        public decimal? PerOperationLimit { get; set; }
    }

    public class RemoveCardUserAccessViewModel
    {
        public int CardId { get; set; }
        public int CardUserProfileId { get; set; }
    }
}
