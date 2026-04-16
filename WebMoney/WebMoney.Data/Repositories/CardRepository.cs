using Microsoft.EntityFrameworkCore;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories;

public class CardRepository(WebContext webContext) : BaseRepository<Card>(webContext), ICardRepository
{
    public List<Card> GetCardsByUserEmail(string normalizedEmail) =>
        webContext.Cards
            .Where(card => card.CardUserProfiles
                .Any(cup => cup.UserProfile.User.Email == normalizedEmail))
            .ToList();

    public Card? GetCardWithUsersById(int cardId) =>
        webContext.Cards
            .Include(c => c.CardUserProfiles)
            .ThenInclude(cup => cup.UserProfile)
            .ThenInclude(up => up.User)
            .FirstOrDefault(c => c.Id == cardId);
}