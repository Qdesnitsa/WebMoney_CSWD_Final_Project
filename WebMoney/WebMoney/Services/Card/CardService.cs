using Microsoft.AspNetCore.Identity;
using WebMoney.Data.Enum.Card;
using WebMoney.Models;
using WebMoney.Persistence;
using WebMoney.Persistence.Entities;
using WebMoney.Persistence.Storage;

namespace WebMoney.Services;

public class CardService(
    IPasswordHasher<Card> passwordHasher,
    ICardRepository cardRepository,
    IUserRepository userRepository) : ICardService
{
    private const int NumberOfYearesNewCardIsValid = 5;
    private const int NumberOfDigitsCardNumber = 16;
    public IQueryable<Card> GetCardsByUserEmail(string email) => cardRepository.GetCardsByUserEmail(email);

    public NewCardPrepareResult PrepareNewCard(string normalizedEmail, NewCardViewModel model)
    {
        var result = new NewCardPrepareResult();
        if (string.IsNullOrEmpty(model.CardNumber) || model.CardNumber.Length != NumberOfDigitsCardNumber)
        {
            result.Errors.Add((model.CardNumber, "Номер карты — 16 цифр, не начинается с 0"));
        }

        RejectNegative(nameof(model.DailyLimit), model.DailyLimit);
        RejectNegative(nameof(model.MonthlyLimit), model.MonthlyLimit);
        RejectNegative(nameof(model.PerOperationLimit), model.PerOperationLimit);

        var userProfile = userRepository.FindByEmail(normalizedEmail);
        if (userProfile is null)
        {
            result.Errors.Add(("", "Пользователь не найден"));
            return result;
        }

        if (result.Errors.Count != 0)
        {
            return result;
        }

        var card = new Card
        {
            Number = model.CardNumber,
            CurrencyCode = model.CurrencyCode,
            CardLimit = new CardLimit
            {
                DailyLimit = model.DailyLimit,
                MonthlyLimit = model.MonthlyLimit,
                PerOperationLimit = model.PerOperationLimit,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userProfile.Email
            },
            PeriodOfValidity = DefaultPeriodOfValidity(),
            CardStatus = CardStatus.Initialized,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userProfile.Email
        };
        card.HashedPinCode = passwordHasher.HashPassword(card, model.PinCode);
        card.UserProfiles.Add(userProfile);
        cardRepository.Create(card);

        return result;

        void RejectNegative(string field, decimal? value)
        {
            if (value is < 0)
            {
                result.Errors.Add((field, "Лимит не может быть отрицательным"));
            }
        }
    }

    public string GenerateCardNumber()
    {
        var d = new char[NumberOfDigitsCardNumber];
        for (var i = 0; i < NumberOfDigitsCardNumber; i++)
            d[i] = (char)('0' + Random.Shared.Next(i == 0 ? 1 : 0, 10));
        return new string(d);
    }

    public DateOnly DefaultPeriodOfValidity() =>
        DateOnly.FromDateTime(DateTime.Today).AddYears(NumberOfYearesNewCardIsValid);
}