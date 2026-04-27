using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application;
using WebMoney.Application.Auth;
using WebMoney.Auth;
using WebMoney.Models;

namespace WebMoney.Controllers;

[AllowAnonymous]
public class SignUpController(IMediator mediator) : Controller
{
    [HttpGet]
    public IActionResult SignUp() => View(new SignUpViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        AuthResult result;
        try
        {
            result = mediator.SendSync(
                new SignUpCommand(model.UserName, model.Email, model.Password, model.ConfirmPassword));
        }
        catch (ValidationException ex)
        {
            foreach (var err in ex.Errors)
            {
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
            }

            return View(model);
        }

        if (!result.Succeeded)
        {
            model.Alerts.Add(result.ErrorMessage!);
            return View(model);
        }

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            CookiePrincipalFactory.CreateFor(result.User!));

        return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
    }
}
