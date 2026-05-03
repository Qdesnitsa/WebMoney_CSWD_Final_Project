using WebMoney.Application.Cards;

namespace WebMoney.Services;

public interface ICardService
{
    IReadOnlyList<UserCardListReadModel>? GetCardsForUser(int userId);
    DateOnly DefaultPeriodOfValidity();
    bool UserMayManageCardUsers(int userId, int cardId);
    bool UserIsCardParticipant(int userId, int cardId);
    bool UserIsCardOwner(int userId, int cardId);
    string GenerateNotExistingCardNumber();
    PrepareNewCardResult AddUserToCard(int currentUserId, int cardId, string userEmail, decimal? dailyLimit, decimal? monthlyLimit, decimal? perOperationLimit, bool grantCanManageUsers);
    PrepareNewCardResult UpdateUserAccess(int currentUserId, int cardId, int cardUserProfileId, decimal? dailyLimit, decimal? monthlyLimit, decimal? perOperationLimit, bool? canManageUsers);
    PrepareNewCardResult RemoveUserAccess(int currentUserId, int cardId, int cardUserProfileId);
    PrepareNewCardResult PrepareNewCard(int userId, NewCardInput input);
    PrepareNewCardResult GetById(int id);
    PrepareNewCardResult GetCardWithUsersAndCardLimitsById(int cardId);
}