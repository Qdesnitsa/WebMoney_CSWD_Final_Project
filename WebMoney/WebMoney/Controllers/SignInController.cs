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
        
        var userProfile = result.UserProfile!;
        HttpContext.Session.SetString(SessionKeys.USERNAME, userProfile.User.UserName);
        HttpContext.Session.SetString(SessionKeys.USEREMAIL, userProfile.User.Email);
        HttpContext.Session.SetString(SessionKeys.USERROLE, userProfile.User.Role.ToString());

        return userProfile.User.Role != Role.User
            ? RedirectToAction(nameof(HomeController.Error), nameof(HomeController).Replace("Controller", ""))
            : RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
    }
}