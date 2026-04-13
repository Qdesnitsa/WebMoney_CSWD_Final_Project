using WebMoney.Data;
using WebMoney.Data.Persistence;
using WebMoney.Persistence.Entities;
using WebMoney.Persistence.Storage;

namespace WebMoney.Persistence;

public class CardRepository(WebContext webContext) : BaseRepository<Card>(webContext), ICardRepository
{
    public List<Card> GetAllCards() => webContext.Cards.ToList();
    public Card? GetCardById(int cardId) => webContext.Cards.FirstOrDefault(c => c.Id == cardId);

    public List<Card> GetCardsByUserEmail(string normalizedEmail) =>
        webContext.Cards.Where(c => c.UserProfiles.Any(u => u.User.Email == normalizedEmail)).ToList();

    public void Create(Card card)
    {
        webContext.Cards.Add(card);
        webContext.SaveChanges();
    }
}