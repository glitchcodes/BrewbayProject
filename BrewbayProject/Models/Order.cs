using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrewbayProject.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("User")]
    public string UserId { get; set; }
    
    public User User { get; set; }
    
    public string DeliveryAddress { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now; // Default value will be the current date and time
    
    public ICollection<OrderItem> Items { get; set; }
}