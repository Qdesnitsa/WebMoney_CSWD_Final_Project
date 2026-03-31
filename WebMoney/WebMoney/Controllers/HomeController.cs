using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebMoney.Models;

namespace WebMoney.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
