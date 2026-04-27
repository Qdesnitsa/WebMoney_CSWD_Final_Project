using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMoney.Auth;

namespace WebMoney.Controllers;

[Authorize(Policy = AuthPolicies.AdminOnly)]
public class AdminController : Controller
{
    [HttpGet]
    public IActionResult Admin() => View();
}
