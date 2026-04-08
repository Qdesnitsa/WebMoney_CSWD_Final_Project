using Microsoft.AspNetCore.Mvc;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Persistence.Storage;

namespace WebMoney.Controllers;

public class CardController(ICardStore cardStore) : Controller
{
    public IActionResult Show()
    {
        var username = HttpContext.Session.GetString(SessionKeys.USERNAME);
        if (string.IsNullOrWhiteSpace(username))
        {
            return RedirectToAction(nameof(SignInController.SignIn), "SignIn");
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

        return View("/Views/Customer/Card.cshtml", cardViewModel);
    }
}