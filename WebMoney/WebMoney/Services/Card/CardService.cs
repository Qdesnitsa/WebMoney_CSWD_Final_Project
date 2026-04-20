using Microsoft.AspNetCore.Identity;
using WebMoney.Data.Enum;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Models;
using WebMoney.ModelTransfer;
using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public class CardService(
    IPasswordHasher<Card> passwordHasher,
    ICardRepository cardRepository,
    IUserProfileRepository userProfileRepository) : ICardService
{
    private const int NumberOfYearsNewCardIsValid = 5;
    private const int NumberOfDigitsCardNumber = 16;
    public List<Card> GetCardsByUserEmail(string email) => cardRepository.GetCardsByUserEmail(email);
    public string GenerateNotExistingCardNumber()
    {
        var cardNumber = GenerateCardNumber();
        while (CheckCardNumberAlreadyExists(cardNumber))
        {
            cardNumber = GenerateCardNumber();
        }
        return cardNumber;
    }
    
    public bool CheckCardNumberAlreadyExists(string cardNumber) =>
        cardRepository.CheckCardNumberAlreadyExists(cardNumber);

    public CardPrepareResult GetById(int id)
    {
        var result = new CardPrepareResult();
        var card = cardRepository.GetById(id);
        if (card is null)
        {
            result.Errors.Add((string.Empty, "Карта не найдена"));
            return result;
        }

        result.Card = card;
        return result;
    }

    public CardPrepareResult PrepareNewCard(string normalizedEmail, NewCardInput input)
    {
        var result = new CardPrepareResult();
        var userProfile = userProfileRepository.FindByEmail(normalizedEmail);
        if (userProfile is null)
        {
            result.Errors.Add((string.Empty, "Пользователь не найден"));
            return result;
        }

        if (string.IsNullOrEmpty(input.CardNumber) || input.CardNumber.Length != NumberOfDigitsCardNumber)
        {
            result.Errors.Add((input.CardNumber, "Номер карты — 16 цифр, не начинается с 0"));
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
            CreatedBy = userProfile.User.Email,
            CardUserProfiles = new HashSet<CardUserProfile>
            {
                new CardUserProfile
                {
                    UserProfileId = userProfile.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userProfile.User.Email,
                    CardLimit = new CardLimit
                    {
                        DailyLimit = input.DailyLimit,
                        MonthlyLimit = input.MonthlyLimit,
                        PerOperationLimit = input.PerOperationLimit,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userProfile.User.Email,
                    }
                }
            },
        };
        card.HashedPinCode = passwordHasher.HashPassword(card, input.PinCode);
        cardRepository.Create(card);

        return result;
    }

    private void RejectNegative(CardPrepareResult result, string field, decimal? value)
    {
        if (value is < 0)
        {
            result.Errors.Add((field, "Лимит не может быть отрицательным"));
        }
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