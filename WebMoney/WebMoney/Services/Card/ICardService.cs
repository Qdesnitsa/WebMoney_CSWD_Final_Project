using WebMoney.Models;
using WebMoney.ModelTransfer;
using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public interface ICardService
{
    List<Card> GetCardsByUserEmail(string email);
    CardPrepareResult PrepareNewCard(string normalizedEmail, NewCardInput input);
    DateOnly DefaultPeriodOfValidity();
    CardPrepareResult GetById(int id);
    public string GenerateNotExistingCardNumber();
    bool CheckCardNumberAlreadyExists(string cardNumber);
}