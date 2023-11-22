using Microsoft.AspNetCore.Identity;

namespace BrewbayProject.Models;

public enum Role
{
    Admin, Customer
}

public class User : IdentityUser
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public Role Role { get; set; }
    
    public ICollection<Order> Orders { get; set; }
}