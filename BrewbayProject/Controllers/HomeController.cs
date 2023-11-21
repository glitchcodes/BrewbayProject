using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BrewbayProject.Models;

namespace BrewbayProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Menu()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
    
    public IActionResult Cart()
    {
        return View();
    }
    
    public IActionResult Orders()
    {
        return View();
    }
    
    public IActionResult Tracker()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}