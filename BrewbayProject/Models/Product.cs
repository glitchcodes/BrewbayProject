using System.ComponentModel.DataAnnotations;
namespace BrewbayProject.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [DataType(DataType.Upload)]
    public string Image { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    public OrderItem? OrderItem { get; set; }
}