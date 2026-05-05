using WebMoney.Data.Entities;

namespace WebMoney.Auth;

public sealed class CardPermissions : ICardPermissions
{
    public bool IsOwner(User user, Card card) =>
        string.Equals(user.Email, card.CreatedBy, StringComparison.OrdinalIgnoreCase);

    private static CardUserProfile? FindCardUserProfile(User user, Card card) =>
        card.CardUserProfiles?.FirstOrDefault(cup => cup.UserId == user.Id);

    public bool MayManageCardUsers(User user, Card card) =>
        IsOwner(user, card) || (FindCardUserProfile(user, card)?.CanManageUsers ?? false);

    public bool IsCardParticipant(User user, Card card) =>
        IsOwner(user, card) || FindCardUserProfile(user, card) is not null;
}
