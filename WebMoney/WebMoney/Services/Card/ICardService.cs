using WebMoney.Application.Cards;
using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public interface ICardService
{
    List<Card> GetCardsByUserId(int userId);
    PrepareNewCardResult PrepareNewCard(int userId, NewCardInput input);
    DateOnly DefaultPeriodOfValidity();
    PrepareNewCardResult GetById(int id);
    public string GenerateNotExistingCardNumber();
    bool CheckCardNumberAlreadyExists(string cardNumber);
}