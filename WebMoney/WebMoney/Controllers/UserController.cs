using Microsoft.AspNetCore.Mvc;
using WebMoney.Models;

namespace WebMoney.Controllers;

public class UserController : Controller
{
    // GET
    public IActionResult Index(int count)
    {
        var viewModels = new List<UserViewModel>
        {
            new UserViewModel { Url = "images/card_wrappers/card2.jpg" },
            new UserViewModel { Url = "images/card_wrappers/card1.jpg" },
            new UserViewModel { Url = "images/card_wrappers/card3.jpg" }
        };
        viewModels = viewModels.Take(count).ToList();
        return View(viewModels);
    }
}