using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application;
using WebMoney.Application.Auth;
using WebMoney.Data.Enum;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;

namespace WebMoney.Controllers;

public class SignInController(IMediator mediator) : Controller
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
        HttpContext.Session.SetString(SessionKeys.USERNAME, user.UserName);
        HttpContext.Session.SetString(SessionKeys.USEREMAIL, user.Email);
        HttpContext.Session.SetString(SessionKeys.USERROLE, user.Role.ToString());

        return user.Role switch
        {
            Role.User => RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", "")),
            Role.Admin => RedirectToAction(nameof(AdminController.Admin), nameof(AdminController).Replace("Controller", "")),
            _ => RedirectToAction(nameof(HomeController.Error), nameof(HomeController).Replace("Controller", ""))
        };
    }
}