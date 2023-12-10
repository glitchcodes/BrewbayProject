using BrewbayProject.Data;
using Microsoft.AspNetCore.Mvc;
using BrewbayProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BrewbayProject.Controllers;

public class AdminController : Controller
{
  private readonly AppDbContext _dbContext;
  private readonly UserManager<User> _userManager;

  public AdminController(AppDbContext dbContext, UserManager<User> userManager)
  {
    _dbContext = dbContext;
    _userManager = userManager;
  }

  public IActionResult Index()
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Login", "Account");
    }

    var userId = _userManager.GetUserId(User);
    var isAdministrator = _dbContext.Administrators.Any(a => a.UserId.Equals(userId));

    if (!isAdministrator)
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
    
    var userId = _userManager.GetUserId(User);
    var isAdministrator = _dbContext.Administrators.Any(a => a.UserId.Equals(userId));

    if (!isAdministrator)
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
    
    var userId = _userManager.GetUserId(User);
    var isAdministrator = _dbContext.Administrators.Any(a => a.UserId.Equals(userId));

    if (!isAdministrator)
    {
      return RedirectToAction("Login", "Account");
    }

    if (!ModelState.IsValid)
    {
      return View();
    }

    _dbContext.Products.Add(product);
    _dbContext.SaveChanges();

    return RedirectToAction("Index");
  }

  [HttpGet]
  public IActionResult EditProduct(int id)
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Login", "Account");
    }
    
    var userId = _userManager.GetUserId(User);
    var isAdministrator = _dbContext.Administrators.Any(a => a.UserId.Equals(userId));

    if (!isAdministrator)
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
  public IActionResult EditProduct(Product updatedProduct)
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Login", "Account");
    }
    
    var userId = _userManager.GetUserId(User);
    var isAdministrator = _dbContext.Administrators.Any(a => a.UserId.Equals(userId));

    if (!isAdministrator)
    {
      return RedirectToAction("Login", "Account");
    }

    if (!ModelState.IsValid)
    {
      return View(updatedProduct);
    }

    Product? product = _dbContext.Products.FirstOrDefault(product => product.Id == updatedProduct.Id);

    if (product == null)
    {
      return NotFound();
    }

    product.Name = updatedProduct.Name;
    product.Description = updatedProduct.Description;
    product.Image = updatedProduct.Image;
    product.Price = updatedProduct.Price;
    _dbContext.SaveChanges();

    return RedirectToAction("Index");
  }

  [HttpGet]
  public IActionResult DeleteProduct(int id)
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Login", "Account");
    }
    
    var userId = _userManager.GetUserId(User);
    var isAdministrator = _dbContext.Administrators.Any(a => a.UserId.Equals(userId));

    if (!isAdministrator)
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
    
    var userId = _userManager.GetUserId(User);
    var isAdministrator = _dbContext.Administrators.Any(a => a.UserId.Equals(userId));

    if (!isAdministrator)
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

  [HttpGet]
  public IActionResult ViewOrders([FromQuery] string selected)
  {
    if (!User.Identity.IsAuthenticated)
    {
      return RedirectToAction("Index", "Home");
    }
    
    var userId = _userManager.GetUserId(User);
    var isAdministrator = _dbContext.Administrators.Any(a => a.UserId.Equals(userId));

    if (!isAdministrator)
    {
      return RedirectToAction("Login", "Account");
    }
    
    var selectedType = selected.IsNullOrEmpty() ? "Active" : selected;
    ViewData["SelectedType"] = selectedType;
        
    var orders = _dbContext.Orders
      .Where(o => o.Status == selectedType.ToLower())
      .Include(o => o.OrderPayment)
      .Include(o => o.OrderItems)
      .ThenInclude(oi => oi.Product);

    return View(orders);
  }
}
