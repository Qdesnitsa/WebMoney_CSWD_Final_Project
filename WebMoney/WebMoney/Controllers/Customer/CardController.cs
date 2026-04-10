using Microsoft.AspNetCore.Mvc;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Persistence.Storage;

namespace WebMoney.Controllers;

public class CardController(ICardStore cardStore) : Controller
{
    public IActionResult Card()
    {
        var username = HttpContext.Session.GetString(SessionKeys.USERNAME);
        if (string.IsNullOrWhiteSpace(username))
        {
            return RedirectToAction(nameof(SignInController.SignIn), nameof(CardController).Replace("Controller", ""));
        }

        var cardViewModel = new CardViewModel
        {
            Cards = cardStore.GetAllCards().Select(c => new CardViewModel
            {
                Id = c.Id,
                Url = c.Url,
                Number = c.Number,
                UserName = username
            }).ToList()
        };

        return View(cardViewModel);
    }
}