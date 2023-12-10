using BrewbayProject.Data;
using Microsoft.AspNetCore.Mvc;
using BrewbayProject.Models;

namespace BrewbayProject.Controllers;

public class AdminController : Controller
{
  private readonly AppDbContext _dbContext;

  public AdminController(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public IActionResult Index()
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Login", "Account");
    }

    return View(_dbContext.Products);
  }

  [HttpGet]
  public IActionResult AddProduct()
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Login", "Account");
    }

    return View();
  }

  [HttpPost]
  public IActionResult AddProduct(Product product)
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Login", "Account");
    }

    if (!ModelState.IsValid)
    {
      return View();
    }

    return View();
  }

  [HttpGet]
  public IActionResult EditProduct(int id)
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Login", "Account");
    }

    if (!ModelState.IsValid)
    {
      return View();
    }

    Product? product = _dbContext.Products.FirstOrDefault(product => product.Id == id);

    if (product == null)
    {
      return NotFound();
    }

    return View(product);
  }

  [HttpPost]
  public IActionResult EditProduct(Product product)
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Login", "Account");
    }

    if (!ModelState.IsValid)
    {
      return View();
    }

    // TODO: save details

    return View(product);
  }

  [HttpGet]
  public IActionResult DeleteProduct(int id)
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Login", "Account");
    }

    Product? product = _dbContext.Products.FirstOrDefault(product => product.Id == id);

    if (product == null)
    {
      return NotFound();
    }

    return View(product);
  }

  [HttpPost]
  [ActionName("DeleteProduct")]
  public IActionResult DeleteProductConfirmed(int id)
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Login", "Account");
    }

    // user cancel operation
    if (id == -1)
    {
      return RedirectToAction("Index");
    }

    Product? product = _dbContext.Products.FirstOrDefault(product => product.Id == id);

    if (product == null)
    {
      return NotFound();
    }

    _dbContext.Products.Remove(product);
    _dbContext.SaveChanges();

    return RedirectToAction("Index");
  }
}
