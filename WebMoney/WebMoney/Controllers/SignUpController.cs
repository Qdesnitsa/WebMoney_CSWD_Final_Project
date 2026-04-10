using Microsoft.AspNetCore.Mvc;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

public class SignUpController(IAuthService authService) : Controller
{
    [HttpGet]
    public IActionResult SignUp() => View(new SignUpViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SignUp(SignUpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = authService.Register(model.UserName, model.Email, model.Password);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(nameof(SignUpViewModel.Email), result.ErrorMessage!);
            return View(model);
        }

        var user = result.User;
        HttpContext.Session.SetString(SessionKeys.USERNAME, user.UserName);
        HttpContext.Session.SetString(SessionKeys.USERROLE, user.Role.ToString());

        return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
    }
}