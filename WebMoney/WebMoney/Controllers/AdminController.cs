using Microsoft.AspNetCore.Mvc;
using WebMoney.Data.Enum;
using WebMoney.Infrastructure.Constants;

namespace WebMoney.Controllers;

public class AdminController : Controller
{
    [HttpGet]
    public IActionResult Admin()
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionKeys.USERNAME)))
        {
            return RedirectToAction(nameof(SignInController.SignIn),
                nameof(SignInController).Replace("Controller", ""));
        }

        if (HttpContext.Session.GetString(SessionKeys.USERROLE) != Role.Admin.ToString())
        {
            return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
        }

        return View();
    }
}