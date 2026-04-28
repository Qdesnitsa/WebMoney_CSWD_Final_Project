using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application;
using WebMoney.Application.Auth;
using WebMoney.Auth;
using WebMoney.Data.Enum;
using WebMoney.Models;

namespace WebMoney.Controllers;

[Authorize]
public class AuthController(IMediator mediator) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult SignIn() => View(new SignInViewModel());

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(SignInViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        AuthResult result;
        try
        {
            result = mediator.SendSync(new SignInCommand(model.Email, model.Password));
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

        var user = result.User!;
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            CookiePrincipalFactory.CreateFor(user));

        return user.Role switch
        {
            Role.User => RedirectToAction(nameof(CardController.Card),
                nameof(CardController).Replace("Controller", "")),
            Role.Admin => RedirectToAction(nameof(AdminController.Admin),
                nameof(AdminController).Replace("Controller", "")),
            _ => RedirectToAction(nameof(HomeController.Error), nameof(HomeController).Replace("Controller", ""))
        };
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult SignUp() => View(new SignUpViewModel());

    [AllowAnonymous]
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(SignIn), nameof(AuthController).Replace("Controller", ""));
    }
}