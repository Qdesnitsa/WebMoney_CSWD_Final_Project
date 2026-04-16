using WebMoney.Data;
using WebMoney.Data.Persistence;
using WebMoney.Persistence.Entities;
using WebMoney.Persistence.Storage;

namespace WebMoney.Persistence;

public class CardRepository(WebContext webContext) : BaseRepository<Card>(webContext), ICardRepository
{
    public List<Card> GetCardsByUserEmail(string normalizedEmail) =>
        webContext.Cards
            .Where(card => card.CardUserProfiles.Any(cup => cup.UserProfile.User.Email == normalizedEmail)).ToList();
    
}