using Microsoft.AspNetCore.Mvc;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.ModelTransfer;
using WebMoney.Services;

namespace WebMoney.Controllers;

public class CardController(ICardService cardService) : Controller
{
    public IActionResult Card()
    {
        var username = HttpContext.Session.GetString(SessionKeys.USERNAME);
        var useremail = HttpContext.Session.GetString(SessionKeys.USEREMAIL);
        if (string.IsNullOrWhiteSpace(username))
        {
            return RedirectToAction(nameof(SignInController.SignIn),
                nameof(SignInController).Replace("Controller", ""));
        }

        var cardViewModel = new CardViewModel
        {
            Cards = cardService.GetCardsByUserEmail(useremail).Select(c => new CardViewModel
            {
                Id = c.Id,
                Number = c.Number,
                UserName = username,
                ValidThru = c.PeriodOfValidity.ToString(),
                UserEmail = c.CreatedBy,
                Balance = c.Balance,
                CurrencyCode = c.CurrencyCode.ToString()
            }).ToList()
        };

        return View(cardViewModel);
    }

    [HttpGet]
    public IActionResult NewCard()
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionKeys.USERNAME)))
        {
            return RedirectToAction(nameof(SignInController.SignIn),
                nameof(SignInController).Replace("Controller", ""));
        }

        var model = new NewCardViewModel
        {
            CardNumber = cardService.GenerateCardNumber(),
            PeriodOfValidity = cardService.DefaultPeriodOfValidity()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult NewCard(NewCardViewModel model)
    {
        var userEmail = HttpContext.Session.GetString(SessionKeys.USEREMAIL);
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionKeys.USERNAME)) ||
            userEmail == null)
        {
            return RedirectToAction(nameof(SignInController.SignIn),
                nameof(SignInController).Replace("Controller", ""));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var input = new NewCardInput
        {
            CardNumber = model.CardNumber,
            CurrencyCode = model.CurrencyCode,
            DailyLimit = model.DailyLimit,
            MonthlyLimit = model.MonthlyLimit,
            PerOperationLimit = model.PerOperationLimit,
            PinCode = model.PinCode
        };

        var normalizedEmail = userEmail.Trim().ToLowerInvariant();
        var result = cardService.PrepareNewCard(normalizedEmail, input);
        foreach (var (field, message) in result.Errors)
        {
            ModelState.AddModelError(string.IsNullOrEmpty(field) ? string.Empty : field, message);
        }

        if (!result.Success)
        {
            return View(model);
        }

        return RedirectToAction(nameof(Card));
    }
}