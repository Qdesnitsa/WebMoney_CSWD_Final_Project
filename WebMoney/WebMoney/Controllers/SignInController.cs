using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Data.Enum;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Services;

namespace WebMoney.Controllers;

public class SignInController(IAuthService authService, IValidator<SignInViewModel> signInValidator) : Controller
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

        var validationResult = signInValidator.Validate(model);
        if (!validationResult.IsValid)
        {
            foreach (var err in validationResult.Errors)
            {
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
            }

            return View(model);
        }

        var result = authService.TrySignIn(model.Email, model.Password);
        if (!result.Succeeded)
        {
            model.Alerts.Add(result.ErrorMessage!);
            return View(model);
        }
        
        var user = result.User!;
        HttpContext.Session.SetString(SessionKeys.USERNAME, user.UserName);
        HttpContext.Session.SetString(SessionKeys.USEREMAIL, user.Email);
        HttpContext.Session.SetString(SessionKeys.USERROLE, user.Role.ToString());

        return user.Role != Role.User
            ? RedirectToAction(nameof(HomeController.Error), nameof(HomeController).Replace("Controller", ""))
            : RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
    }
}