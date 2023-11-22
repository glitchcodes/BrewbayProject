using BrewbayProject.Models;
using BrewbayProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BrewbayProject.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        // Redirect to menu page if logged in
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Menu", "Home");
        }
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel data)
    {
        var result = await _signInManager.PasswordSignInAsync(data.Username, data.Password, data.RememberMe, false);

        if (result.Succeeded)
        {
            return RedirectToAction("Menu", "Home");
        }
        else
        {
            ModelState.AddModelError("", "Incorrect credentials! Please try again.");
        }

        return View(data);
    }

    [HttpGet]
    public IActionResult Register()
    {
        // Redirect to menu page if logged in
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Menu", "Home");
        }
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel data)
    {
        if (ModelState.IsValid)
        {
            User user = new User();
            user.UserName = data.UserName;
            user.FirstName = data.FirstName;
            user.LastName = data.LastName;
            user.Email = data.Email;

            var result = await _userManager.CreateAsync(user, data.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }

        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}