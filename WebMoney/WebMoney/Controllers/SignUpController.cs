using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Application;
using WebMoney.Application.Auth;
using WebMoney.Data.Enum;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;

namespace WebMoney.Controllers;

public class SignUpController(IMediator mediator) : Controller
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
        
        HttpContext.Session.SetString(SessionKeys.USERNAME, model.UserName);
        HttpContext.Session.SetString(SessionKeys.USEREMAIL, model.Email);
        HttpContext.Session.SetString(SessionKeys.USERROLE, Role.User.ToString());

        return RedirectToAction(nameof(CardController.Card), nameof(CardController).Replace("Controller", ""));
    }
}