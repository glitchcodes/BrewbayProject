using System.Diagnostics;
using BrewbayProject.Data;
using Microsoft.AspNetCore.Mvc;
using BrewbayProject.Models;

namespace BrewbayProject.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _dbContext;
    
    public HomeController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Menu()
    {
        return View(_dbContext.Products);
    }

    public IActionResult About()
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
}