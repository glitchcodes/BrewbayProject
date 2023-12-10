using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrewbayProject.Models;

public enum Size
{
    Iced, Hot
}

public class OrderItem
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("Order")]
    public int OrderId { get; set; }
    
    public Order Order { get; set; }
    
    [ForeignKey("Product")]
    public int ProductId { get; set; }

    public Product Product { get; set; }
    
    public int Quantity { get; set; }
    
    public Size Size { get; set; }
}