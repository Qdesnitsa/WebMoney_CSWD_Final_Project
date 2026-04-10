using Microsoft.AspNetCore.Mvc;
using WebMoney.Enum;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

public class SignInController(IAuthService authService) : Controller
{
    [HttpGet]
    public IActionResult SignIn() => View(new SignInViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SignIn(SignInViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = authService.TrySignIn(model.Email, model.Password);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return View(model);
        }

        var user = result.User!;
        HttpContext.Session.SetString(SessionKeys.USERNAME, user.UserName);
        HttpContext.Session.SetString(SessionKeys.USERROLE, user.Role.ToString());

        return user.Role != Role.User
            ? RedirectToAction(nameof(HomeController.Error), nameof(HomeController).Replace("Controller", ""))
            : RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
    }
}