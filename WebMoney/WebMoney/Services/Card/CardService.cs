using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using WebMoney.Auth;
using WebMoney.Data.Enum;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Application.Cards;
using WebMoney.Models;
using WebMoney.Data.Entities;

namespace WebMoney.Services;

public class CardService(
    IPasswordHasher<Card> passwordHasher,
    ICardRepository cardRepository,
    IUserRepository userRepository,
    IStringLocalizer<SharedResource> localizer,
    ICardPermissions permissions) : ICardService
{
    private const int NumberOfYearsNewCardIsValid = 5;
    private const int NumberOfDigitsCardNumber = 16;

    public IReadOnlyList<UserCardListReadModel>? GetCardsForUser(int userId)
    {
        var user = userRepository.GetById(userId);
        if (user is null)
        {
            return null;
        }

        var cards = cardRepository.GetCardsByUserId(userId);
        var list = new List<UserCardListReadModel>(cards.Count);
        foreach (var card in cards)
        {
            list.Add(new UserCardListReadModel
            {
                CardId = card.Id,
                MaskedNumber = CardNumberMask.Mask(card.Number),
                ValidThru = card.PeriodOfValidity.ToString(),
                CreatedBy = card.CreatedBy,
                Balance = card.Balance,
                CurrencyCode = card.CurrencyCode.ToString(),
                ShowUserManagement = permissions.MayManageCardUsers(user, card),
                IsOwner = permissions.IsOwner(user, card),
            });
        }

        return list;
    }

    public string GenerateNotExistingCardNumber()
    {
        var cardNumber = GenerateCardNumber();
        while (CardNumberAlreadyExists(cardNumber))
        {
            cardNumber = GenerateCardNumber();
        }

        return cardNumber;
    }

    private bool CardNumberAlreadyExists(string cardNumber) =>
        cardRepository.CheckCardNumberAlreadyExists(cardNumber);

    public PrepareNewCardResult GetById(int id)
    {
        var result = new PrepareNewCardResult();
        var card = cardRepository.GetById(id);
        if (card is null)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_CardNotFound"].Value!));
            return result;
        }

        result.Card = card;
        return result;
    }

    public PrepareNewCardResult GetCardWithUsersAndCardLimitsById(int cardId)
    {
        var result = new PrepareNewCardResult();
        var card = cardRepository.GetCardWithUsersAndCardLimitsById(cardId);
        if (card is null)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_CardNotFound"].Value!));
            return result;
        }

        result.Card = card;
        return result;
    }

    public bool UserMayManageCardUsers(int userId, int cardId)
    {
        var user = userRepository.GetById(userId);
        if (user is null)
        {
            return false;
        }

        var card = cardRepository.GetCardWithUsersAndCardLimitsById(cardId);
        if (card is null)
        {
            return false;
        }

        return permissions.MayManageCardUsers(user, card);
    }

    public bool UserIsCardParticipant(int userId, int cardId)
    {
        var user = userRepository.GetById(userId);
        if (user is null)
        {
            return false;
        }
        var card = cardRepository.GetCardWithUsersAndCardLimitsById(cardId);
        if (card is null)
        {
            return false;
        }
        return permissions.IsCardParticipant(user, card);
    }

    public bool UserIsCardOwner(int userId, int cardId)
    {
        var user = userRepository.GetById(userId);
        if (user is null)
        {
            return false;
        }
        var card = cardRepository.GetCardWithUsersAndCardLimitsById(cardId);
        if (card is null)
        {
            return false;
        }
        return permissions.IsOwner(user, card);
    }

    public string? GetIdentityDocumentPhotoLink(int userId)
    {
        var userProfile = userRepository.GetProfileWithIdentityDocumentByUserId(userId);
        return userProfile?.IdentityDocument?.DocumentPhotoLink;
    }

    public UploadIdentityDocumentResult UploadIdentityDocumentPhoto(int userId, string documentPhotoLink)
    {
        var result = new UploadIdentityDocumentResult();
        var userProfile = userRepository.GetProfileWithIdentityDocumentByUserId(userId);
        if (userProfile?.User is null)
        {
            result.Errors.Add(localizer["Service_Err_UserNotFound"].Value!);
            return result;
        }

        if (userProfile.IdentityDocument is null)
        {
            var identityDocument = new IdentityDocument
            {
                FirstName = string.Empty,
                LastName = string.Empty,
                DocumentIDNumber = string.Empty,
                DocumentPhotoLink = documentPhotoLink,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userProfile.User.Email
            };
            userProfile.IdentityDocument = identityDocument;
        }
        else
        {
            userProfile.IdentityDocument.DocumentPhotoLink = documentPhotoLink;
            userProfile.IdentityDocument.UpdatedAt = DateTime.UtcNow;
            userProfile.IdentityDocument.UpdatedBy = userProfile.User.Email;
        }

        userProfile.UpdatedAt = DateTime.UtcNow;
        userProfile.UpdatedBy = userProfile.User.Email;
        userRepository.SaveChanges();
        return result;
    }

    public PrepareNewCardResult AddUserToCard(
        int currentUserId,
        int cardId,
        string userEmail,
        decimal? dailyLimit,
        decimal? monthlyLimit,
        decimal? perOperationLimit,
        bool grantCanManageUsers)
    {
        var result = new PrepareNewCardResult();
        var currentUser = userRepository.GetById(currentUserId);
        if (currentUser is null)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_UserNotFound"].Value!));
            return result;
        }

        var card = cardRepository.GetCardWithUsersAndCardLimitsById(cardId);
        if (card is null)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_CardNotFound"].Value!));
            return result;
        }

        if (!permissions.MayManageCardUsers(currentUser, card))
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_NoPermissionManageCardUsers"].Value!));
            return result;
        }

        var normalizedEmail = NormalizeEmail(userEmail);
        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            result.Errors.Add((nameof(userEmail), localizer["Service_Err_SpecifyUserEmail"].Value!));
            return result;
        }

        var user = userRepository.FindByEmail(normalizedEmail);
        if (user is null)
        {
            result.Errors.Add((nameof(userEmail), localizer["Service_Err_UserEmailNotFound"].Value!));
            return result;
        }

        if (card.CardUserProfiles.Any(cup => cup.UserId == user.Id))
        {
            result.Errors.Add((nameof(userEmail), localizer["Service_Err_UserAlreadyHasCardAccess"].Value!));
            return result;
        }

        RejectNegative(result, nameof(dailyLimit), dailyLimit);
        RejectNegative(result, nameof(monthlyLimit), monthlyLimit);
        RejectNegative(result, nameof(perOperationLimit), perOperationLimit);
        if (!result.Success)
        {
            return result;
        }

        card.CardUserProfiles.Add(new CardUserProfile
        {
            UserId = user.Id,
            CardId = card.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUser.Email,
            CanManageUsers = grantCanManageUsers,
            CardLimit = CreateLimitIfNeeded(currentUser.Email, dailyLimit, monthlyLimit, perOperationLimit)
        });

        cardRepository.SaveChanges();
        result.Card = card;
        return result;
    }

    public PrepareNewCardResult UpdateUserAccess(
        int currentUserId,
        int cardId,
        int cardUserProfileId,
        decimal? dailyLimit,
        decimal? monthlyLimit,
        decimal? perOperationLimit,
        bool? canManageUsers)
    {
        var result = new PrepareNewCardResult();
        var currentUser = userRepository.GetById(currentUserId);
        if (currentUser is null)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_UserNotFound"].Value!));
            return result;
        }

        var card = cardRepository.GetCardWithUsersAndCardLimitsById(cardId);
        if (card is null)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_CardNotFound"].Value!));
            return result;
        }

        if (!permissions.MayManageCardUsers(currentUser, card))
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_NoPermissionManageCardUsers"].Value!));
            return result;
        }

        var cardUserProfile = card.CardUserProfiles.FirstOrDefault(cup => cup.Id == cardUserProfileId);
        if (cardUserProfile is null)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_CardUserNotFound"].Value!));
            return result;
        }

        RejectNegative(result, nameof(dailyLimit), dailyLimit);
        RejectNegative(result, nameof(monthlyLimit), monthlyLimit);
        RejectNegative(result, nameof(perOperationLimit), perOperationLimit);

        if (!result.Success)
        {
            return result;
        }

        var targetIsOwner =
            string.Equals(cardUserProfile.User?.Email, card.CreatedBy, StringComparison.OrdinalIgnoreCase);

        if (targetIsOwner)
        {
            cardUserProfile.CanManageUsers = true;
        }
        else if (permissions.MayManageCardUsers(currentUser, card) && canManageUsers.HasValue)
        {
            cardUserProfile.CanManageUsers = canManageUsers.Value;
        }

        if (cardUserProfile.CardLimit is null)
        {
            cardUserProfile.CardLimit =
                CreateLimitIfNeeded(currentUser.Email, dailyLimit, monthlyLimit, perOperationLimit);
        }
        else
        {
            cardUserProfile.CardLimit.DailyLimit = dailyLimit;
            cardUserProfile.CardLimit.MonthlyLimit = monthlyLimit;
            cardUserProfile.CardLimit.PerOperationLimit = perOperationLimit;
            cardUserProfile.CardLimit.UpdatedAt = DateTime.UtcNow;
            cardUserProfile.CardLimit.UpdatedBy = currentUser.Email;
        }

        cardUserProfile.UpdatedAt = DateTime.UtcNow;
        cardUserProfile.UpdatedBy = currentUser.Email;

        cardRepository.SaveChanges();
        result.Card = card;
        return result;
    }

    public PrepareNewCardResult RemoveUserAccess(int currentUserId, int cardId, int cardUserProfileId)
    {
        var result = new PrepareNewCardResult();
        var currentUser = userRepository.GetById(currentUserId);
        if (currentUser is null)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_UserNotFound"].Value!));
            return result;
        }

        var card = cardRepository.GetCardWithUsersAndCardLimitsById(cardId);
        if (card is null)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_CardNotFound"].Value!));
            return result;
        }

        if (!permissions.MayManageCardUsers(currentUser, card))
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_NoPermissionManageCardUsers"].Value!));
            return result;
        }

        var cardUserProfile = card.CardUserProfiles.FirstOrDefault(cup => cup.Id == cardUserProfileId);
        if (cardUserProfile is null)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_CardUserNotFound"].Value!));
            return result;
        }

        if (string.Equals(cardUserProfile.User?.Email, card.CreatedBy, StringComparison.OrdinalIgnoreCase))
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_CannotDetachOwner"].Value!));
            return result;
        }

        cardRepository.RemoveCardUserProfileWithOptionalLimit(card, cardUserProfile);
        result.Card = card;
        return result;
    }

    public PrepareNewCardResult PrepareNewCard(int userId, NewCardInput input)
    {
        var result = new PrepareNewCardResult();
        var user = userRepository.GetById(userId);
        if (user is null)
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_UserNotFound"].Value!));
            return result;
        }

        var existingCards = cardRepository.GetCardsByUserId(userId);
        if (existingCards.Count > 0 && !existingCards.Any(c => permissions.IsOwner(user, c)))
        {
            result.Errors.Add((string.Empty, localizer["Service_Err_NewCardOwnerOnly"].Value!));
            return result;
        }

        if (string.IsNullOrEmpty(input.CardNumber) || input.CardNumber.Length != NumberOfDigitsCardNumber)
        {
            result.Errors.Add((input.CardNumber, localizer["Service_Err_CardNumberFormat"].Value!));
        }

        RejectNegative(result, nameof(input.DailyLimit), input.DailyLimit);
        RejectNegative(result, nameof(input.MonthlyLimit), input.MonthlyLimit);
        RejectNegative(result, nameof(input.PerOperationLimit), input.PerOperationLimit);

        if (result.Errors.Count != 0)
        {
            return result;
        }

        var card = new Card
        {
            Number = input.CardNumber,
            CurrencyCode = input.CurrencyCode,
            PeriodOfValidity = DefaultPeriodOfValidity(),
            CardStatus = CardStatus.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user.Email,
            CardUserProfiles = new HashSet<CardUserProfile>
            {
                new CardUserProfile
                {
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = user.Email,
                    CanManageUsers = true,
                    CardLimit = new CardLimit
                    {
                        DailyLimit = input.DailyLimit,
                        MonthlyLimit = input.MonthlyLimit,
                        PerOperationLimit = input.PerOperationLimit,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = user.Email,
                    }
                }
            },
        };
        card.HashedPinCode = passwordHasher.HashPassword(card, input.PinCode);
        cardRepository.Create(card);

        return result;
    }

    private void RejectNegative(PrepareNewCardResult result, string field, decimal? value)
    {
        if (value is < 0)
        {
            result.Errors.Add((field, localizer["Service_Err_LimitNonNegative"].Value!));
        }
    }

    private static string NormalizeEmail(string? email) =>
        email?.Trim().ToLowerInvariant() ?? string.Empty;

    private static CardLimit? CreateLimitIfNeeded(string createdBy, decimal? dailyLimit, decimal? monthlyLimit,
        decimal? perOperationLimit)
    {
        if (dailyLimit is null && monthlyLimit is null && perOperationLimit is null)
        {
            return null;
        }

        return new CardLimit
        {
            DailyLimit = dailyLimit,
            MonthlyLimit = monthlyLimit,
            PerOperationLimit = perOperationLimit,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
        };
    }

    private string GenerateCardNumber()
    {
        var cardNumbers = new char[NumberOfDigitsCardNumber];
        for (var i = 0; i < NumberOfDigitsCardNumber; i++)
        {
            cardNumbers[i] = (char)('0' + Random.Shared.Next(i == 0 ? 1 : 0, 10));
        }

        return new string(cardNumbers);
    }

    public DateOnly DefaultPeriodOfValidity() =>
        DateOnly.FromDateTime(DateTime.Today).AddYears(NumberOfYearsNewCardIsValid);
}