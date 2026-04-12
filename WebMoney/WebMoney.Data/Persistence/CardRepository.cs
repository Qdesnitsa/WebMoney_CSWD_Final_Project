using WebMoney.Data;
using WebMoney.Persistence.Entities;
using WebMoney.Persistence.Storage;

namespace WebMoney.Persistence;

public class CardRepository(WebContext webContext) : ICardRepository
{
    public IQueryable<Card> GetAllCards() => webContext.Cards;
    public Card? GetCardById(int cardId) => webContext.Cards.FirstOrDefault(c => c.Id == cardId);
    public IQueryable<Card> GetCardsByUserEmail(string normalizedEmail) => 
        webContext.Cards.Where(c => c.UserProfiles.Any(u => u.Email == normalizedEmail));

    public void Create(Card card)
    {
        webContext.Cards.Add(card);
        webContext.SaveChanges();
    }
}