using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrewbayProject.Models;

public class OrderPayment
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Order")]
    public int OrderId { get; set; }
    
    public string PaymongoPaymentId { get; set; }
    
    public decimal AmountPaid { get; set; }
    public decimal Fee { get; set; }
    public decimal NetAmount { get; set; }
    public string PaymentProvider { get; set; }
}