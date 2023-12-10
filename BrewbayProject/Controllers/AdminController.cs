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
    return View(_dbContext.Products);
  }

  [HttpGet]
  public IActionResult Add()
  {
    return View();
  }

  [HttpPost]
  public IActionResult Add(Product product)
  {
    if (!ModelState.IsValid)
    {
      return View();
    }

    return View();
  }

  [HttpGet]
  public IActionResult Edit(int id)
  {
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
  public IActionResult Edit(Product product)
  {
    if (!ModelState.IsValid)
    {
      return View();
    }

    // save here

    return RedirectToAction("Index");
  }

  [HttpGet]
  public IActionResult Delete(int id)
  {
    Product? product = _dbContext.Products.FirstOrDefault(product => product.Id == id);

    if (product == null)
    {
      return NotFound();
    }

    return View(product);
  }

  [HttpPost]
  public IActionResult DeleteProduct(int id)
  {
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
