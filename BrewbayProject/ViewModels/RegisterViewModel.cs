using System.ComponentModel.DataAnnotations;

namespace BrewbayProject.ViewModels;

public class RegisterViewModel
{
    [Display(Name = "First name")]
    public string? FirstName { get; set; }
    
    [Display(Name = "Last name")]
    public string? LastName { get; set; }
    
    [Display(Name = "Username")]
    [Required(ErrorMessage = "Username is required")]
    public string? UserName { get; set; }
    
    [Display(Name = "Email address")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email address is required")]
    public string? Email { get; set; }
    
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
    
    [Display(Name = "Confirm password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password confirmation is required")]
    public string? ConfirmPassword { get; set; }
}