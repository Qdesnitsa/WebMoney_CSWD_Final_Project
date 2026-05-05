using WebMoney.Data.Entities;

namespace WebMoney.Auth;

public interface ICardPermissions
{
    bool IsOwner(User user, Card card);
    bool MayManageCardUsers(User user, Card card);
    bool IsCardParticipant(User user, Card card);
}
