using Microsoft.AspNetCore.Mvc;
using WebMoney.Enum;
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
        
        HttpContext.Session.SetString(SessionKeys.USERNAME, model.UserName);
        HttpContext.Session.SetString(SessionKeys.USEREMAIL, model.Email);
        HttpContext.Session.SetString(SessionKeys.USERROLE, Role.User.ToString());

        return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
    }
}