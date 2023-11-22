using System.ComponentModel.DataAnnotations;

namespace BrewbayProject.ViewModels;

public class LoginViewModel
{
    [Display(Name = "Username")]
    [Required(ErrorMessage = "Username is required")]
    public string? Username { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
    
    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}