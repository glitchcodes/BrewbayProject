namespace BrewbayProject.Models;

public class CartItem
{
    public Product Product { get; set; }
    
    public string? Size { get; set; }
    public int Quantity { get; set; }

    private decimal _SubTotal;

    public decimal SubTotal
    {
        get { return _SubTotal; }
        set { _SubTotal = Product.Price * Quantity; }
    }
}