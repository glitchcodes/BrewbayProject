using System.Diagnostics;
using System.Text;
using System.Text.Json;
using BrewbayProject.Data;
using BrewbayProject.Data.Paymongo;
using BrewbayProject.Extensions;
using BrewbayProject.Helpers;
using BrewbayProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace BrewbayProject.Controllers;

public class CartController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    private List<CartItem> _cart;

    public CartController(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;

        _cart = HttpContext.Session.Get<List<CartItem>>("cart");
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");

        if (cart != null)
        {
            ViewBag.total = cart.Sum(s => s.Quantity * s.Product.Price);
        }
        else
        {
            cart = new List<CartItem>();
            ViewBag.total = 0;
        }
        
        return View(cart);
    }

    [HttpPost]
    public IActionResult AddToCart(int Id, string size)
    {
        var product = _dbContext.Products.FirstOrDefault(p => p.Id == Id);
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");

        if (cart == null)
        {
            cart = new List<CartItem>();
            cart.Add(
                new CartItem
                {
                    Product = product, Quantity = 1, Size = size
                }
            );
        }
        else
        {
            int index = cart.FindIndex(p => p.Product.Id == Id);
            
            if (index != -1)
            {
                cart[index].Quantity++;
            }
            else
            {
                cart.Add(
                    new CartItem
                    {
                        Product = product, Quantity = 1
                    }
                );
            }
        }
        
        HttpContext.Session.Set<List<CartItem>>("cart", cart);
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public IActionResult IncrementItem(int id)
    {
        var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");

        int index = cart.FindIndex(p => p.Product.Id == id);
        cart[index].Quantity++;
        
        HttpContext.Session.Set<List<CartItem>>("cart", cart);
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public IActionResult DecrementItem(int id)
    {
        var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");

        int index = cart.FindIndex(p => p.Product.Id == id);

        // Remove item if the current quantity is 1
        if (cart[index].Quantity == 1)
        {
            cart.RemoveAt(index);
        }
        else // Decrement the quantity instead
        {
            cart[index].Quantity--;
        }
        
        HttpContext.Session.Set<List<CartItem>>("cart", cart);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult RemoveItem(int id)
    {
        var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");
        
        int index = cart.FindIndex(p => p.Product.Id == id);
        cart.RemoveAt(index);
        
        HttpContext.Session.Set<List<CartItem>>("cart", cart);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Checkout()
    {
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");
        var httpClient = PaymongoApiHelper.GetHttpClient(_configuration);
        
        var lineItems = new List<LineItem>();
        
        foreach (var t in cart)
        {
            lineItems.Add(
                new LineItem
                {
                    name = t.Product.Name,
                    description = t.Product.Description,
                    amount = (int) t.Product.Price * 100,
                    currency = "PHP",
                    quantity = t.Quantity,
                    images = new List<string>
                    {
                        t.Product.Image
                    }
                }
            );
        }

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new
            {
                data = new PaymongoData
                {
                    attributes = new Attributes
                    {
                        show_description = true,
                        show_line_items = true,
                        send_email_receipt = false,
                        description = "Test",
                        payment_method_types = new List<string>
                        {
                            "paymaya",
                            "gcash"
                        },
                        line_items = lineItems
                    }
                }
            }),
            Encoding.UTF8,
            "application/json"
        );
        
        var response = await httpClient.PostAsync("checkout_sessions", jsonContent);
        
        if (response.IsSuccessStatusCode)
        {
            string stateInfo = response.Content.ReadAsStringAsync().Result;
        }

        return NotFound();
    }
}