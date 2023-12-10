using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrewbayProject.Models;

public enum OrderStatus
{
    Pending, Canceled, Paid, Failed
}

public class Order
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string ReferenceId { get; set; }
    public string PaymongoCheckoutId { get; set; }
    public string DeliveryAddress { get; set; }
    public string Status { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now; // Default value will be the current date and time
    
    public User User { get; set; }
    public ICollection<OrderItem> Items { get; set; }
}