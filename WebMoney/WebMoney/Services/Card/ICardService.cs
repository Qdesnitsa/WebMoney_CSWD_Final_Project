using WebMoney.Models;
using WebMoney.ModelTransfer;
using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public interface ICardService
{
    List<Card> GetCardsByUserEmail(string email);
    CardPrepareResult PrepareNewCard(string normalizedEmail, NewCardInput input);
    string GenerateCardNumber();
    DateOnly DefaultPeriodOfValidity();
    CardPrepareResult GetById(int id);
}