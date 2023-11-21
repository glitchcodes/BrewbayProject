using Microsoft.AspNetCore.Mvc;

namespace BrewbayProject.Controllers;

public class LoginController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}