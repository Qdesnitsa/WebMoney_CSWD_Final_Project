using Microsoft.AspNetCore.Mvc;

namespace WebMoney.Controllers;

public class SignOutController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SignOut()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(SignInController.SignIn), nameof(SignInController).Replace("Controller", ""));
    }
}