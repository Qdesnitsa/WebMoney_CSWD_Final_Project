using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Enum;
using WebMoney.Infrastructure.Constants;
using WebMoney.Models;
using WebMoney.Persistence;
using WebMoney.Persistence.Entities;

namespace WebMoney.Controllers;

public class SignUpController(IPasswordHasher<User> passwordHasher, IUserStore userStore) : Controller
{
    [HttpGet]
    public IActionResult SignUp() => View(new SignUpViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SignUp(SignUpViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();

        if (userStore.GetAllUsers().Any(u => u.Email == normalizedEmail))
        {
            ModelState.AddModelError(nameof(SignUpViewModel.Email), "Пользователь с таким email уже зарегистрирован");
            return View(model);
        }

        var user = new User { Email = normalizedEmail, Role = Role.User, UserName = model.UserName };
        user.HashedPassword = passwordHasher.HashPassword(user, model.Password);
        userStore.Create(user);

        HttpContext.Session.SetString(SessionKeys.USERNAME, user.UserName);
        HttpContext.Session.SetString(SessionKeys.USERROLE, user.Role.ToString());

        return RedirectToAction(nameof(CardController.Show), "Card");
    }
}