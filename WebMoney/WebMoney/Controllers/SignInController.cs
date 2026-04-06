using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Enum;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Persistence;
using WebMoney.Persistence.Entities;

namespace WebMoney.Controllers;

public class SignInController(IPasswordHasher<User> passwordHasher, IUserStore userStore) : Controller
{
    [HttpGet]
    public IActionResult SignIn() => View("SignIn", new SignInViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SignIn(SignInViewModel model)
    {
        if (!ModelState.IsValid)
            return View("SignIn", model);

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        var user = userStore.GetAllUsers().FirstOrDefault(u => u.Email == normalizedEmail);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Неверный email или пароль");
            return View("SignIn", model);
        }

        var verify = passwordHasher.VerifyHashedPassword(user, user.HashedPassword, model.Password);
        if (verify != PasswordVerificationResult.Success &&
            verify != PasswordVerificationResult.SuccessRehashNeeded)
        {
            ModelState.AddModelError(string.Empty, "Неверный email или пароль");
            return View("SignIn", model);
        }

        if (verify == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.HashedPassword = passwordHasher.HashPassword(user, model.Password);
        }

        HttpContext.Session.SetString(SessionKeys.USERNAME, user.UserName);
        HttpContext.Session.SetString(SessionKeys.USERROLE, user.Role.ToString());

        if (user.Role != Role.User) return RedirectToAction("Error", "Home");

        return RedirectToAction("Index", "Card");
    }
}