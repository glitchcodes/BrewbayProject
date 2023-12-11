using System.Diagnostics;
using System.Text;
using BrewbayProject.Data;
using BrewbayProject.Data.Paymongo;
using BrewbayProject.Extensions;
using BrewbayProject.Helpers;
using BrewbayProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BrewbayProject.Controllers;

public class CartController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public CartController(AppDbContext dbContext, UserManager<User> userManager, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }
        
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
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var product = _dbContext.Products.FirstOrDefault(p => p.Id == Id);
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");
        var sizeEnum = (Size) Enum.Parse(typeof(Size), size);

        if (cart == null)
        {
            cart = new List<CartItem>();
            cart.Add(
                new CartItem
                {
                    Product = product, Quantity = 1, Size = sizeEnum
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
                        Product = product, Quantity = 1, Size = sizeEnum
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
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }
        
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
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }
        
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
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");
        
        int index = cart.FindIndex(p => p.Product.Id == id);
        cart.RemoveAt(index);
        
        HttpContext.Session.Set<List<CartItem>>("cart", cart);
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = _userManager.GetUserId(User);
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");

        // Redirect user to homepage if not logged in
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        
        // If cart is empty, return to cart page
        // TODO: Add error message
        if (cart.IsNullOrEmpty())
        {
            return RedirectToAction("Index");
        }
        
        var httpClient = PaymongoApiHelper.GetHttpClient(_configuration);
        
        // Paymongo
        var referenceId = GenerateReferenceId(20);
        var baseUrl = Request.Scheme + "://" + Request.Host;
        var successUrl = baseUrl + $"/Cart/PaymentSuccess?refId={referenceId}";
        var cancelUrl = baseUrl + $"/Cart/PaymentCanceled?refId={referenceId}";
        
        var lineItems = new List<LineItem>();
        
        foreach (var t in cart)
        {
            lineItems.Add(
                new LineItem
                {
                    name = t.Product.Name,
                    description = t.Size.ToString(),
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
                        line_items = lineItems,
                        success_url = successUrl,
                        cancel_url = cancelUrl
                    }
                }
            }),
            Encoding.UTF8,
            "application/json"
        );
        
        var response = await httpClient.PostAsync("checkout_sessions", jsonContent);
        // Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
        if (response.IsSuccessStatusCode)
        {
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var convertedJson = JsonConvert.DeserializeObject<dynamic>(jsonString)!;
            var checkoutUrl = convertedJson.data.attributes.checkout_url.ToString() as string;

            var order = new Order
            {
                UserId = userId,
                ReferenceId = referenceId,
                PaymongoCheckoutId = convertedJson.data.id.ToString() as string ?? string.Empty,
                DeliveryAddress = "",
                Status = "pending"
            };

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            foreach (var p in cart)
            {
                var item = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = p.Product.Id,
                    Quantity = p.Quantity,
                    Size = p.Size
                };

                _dbContext.OrderItems.Add(item);
                _dbContext.SaveChanges();
            }
            
            return Redirect(checkoutUrl!);
        }

        return NotFound();
    }

    public async Task<IActionResult> PaymentSuccess([FromQuery] string refId)
    {
        if (refId.IsNullOrEmpty())
        {
            return RedirectToAction("Index", "Home");
        }
        
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");

        var httpClient = PaymongoApiHelper.GetHttpClient(_configuration);
        var order = _dbContext.Orders.FirstOrDefault(o => o.ReferenceId == refId);

        if (order != null)
        {
            // Check if order already has payment
            var payment = _dbContext.OrderPayments.FirstOrDefault(p => p.OrderId == order.Id);

            // Call Paymongo API for status
            var response = await httpClient.GetAsync($"checkout_sessions/{order.PaymongoCheckoutId}");
            dynamic convertedJson = "";
            dynamic payments = "";

            if (response.IsSuccessStatusCode)
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                convertedJson = JsonConvert.DeserializeObject<dynamic>(jsonString)!;
                payments = convertedJson.data.attributes.payments;
            }
            
            // If not, store payment info
            if (payment == null)
            {
                payment = new OrderPayment
                {
                    OrderId = order.Id,
                    PaymongoPaymentId = convertedJson.data.id,
                    AmountPaid = ConvertToDecimal(Int16.Parse(payments[0].attributes.amount.ToString() as string)),
                    Fee = ConvertToDecimal(Int16.Parse(payments[0].attributes.fee.ToString() as string)),
                    NetAmount = ConvertToDecimal(Int16.Parse(payments[0].attributes.net_amount.ToString() as string)),
                    PaymentProvider = payments[0].attributes.source.type
                };
                _dbContext.OrderPayments.Add(payment);
                    
                // Update orders
                order.Status = convertedJson.data.attributes.status;
                    
                await _dbContext.SaveChangesAsync();
                    
                // Clear the cart
                cart.Clear();
                HttpContext.Session.Set<List<CartItem>>("cart", cart);
            }
            
            Thread.Sleep(1000);
            
            // Set view bag
            TempData["ReferenceId"] = order.ReferenceId;
            TempData["AmountPaid"] = ConvertToDecimal(Int16.Parse(payments[0].attributes.amount.ToString() as string));
            TempData["PaymentProvider"] = payments[0].attributes.source.type;

            return View(order);
        }

        return NotFound();
    }

    [HttpGet]
    public IActionResult PaymentCanceled([FromQuery] string refId)
    {
        if (refId.IsNullOrEmpty())
        {
            return RedirectToAction("Index", "Home");
        }
        
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");
        var order = _dbContext.Orders.FirstOrDefault(o => o.ReferenceId == refId);

        if (order != null)
        {
            order.Status = "canceled";
            _dbContext.SaveChanges();
            
            // Clear cart
            if (!cart.IsNullOrEmpty())
            {
                cart.Clear();
                HttpContext.Session.Set<List<CartItem>>("cart", cart);
            }

            return View(order);
        }

        return NotFound();
    }

    private string GenerateReferenceId(int length)
    {
        Random random = new Random();
        
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    
    private decimal ConvertToDecimal(int x)
    {
        return x/(decimal)Math.Pow(10.00, 2);
    }

}