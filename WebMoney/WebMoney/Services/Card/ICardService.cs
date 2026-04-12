using WebMoney.Models;
using WebMoney.Persistence.Entities;

namespace WebMoney.Services;

public interface ICardService
{
    IQueryable<Card> GetCardsByUserEmail(string email);
    NewCardPrepareResult PrepareNewCard(string normalizedEmail, NewCardViewModel model);
    string GenerateCardNumber();
    public DateOnly DefaultPeriodOfValidity();
}