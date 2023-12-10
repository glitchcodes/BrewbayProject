using BrewbayProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BrewbayProject.Data;

public class AppDbContext: IdentityDbContext<User>
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public DbSet<OrderPayment> OrderPayments { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Each user has many orders
        builder.Entity<User>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .IsRequired();
        
        // Each order has one assigned user
        builder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(o => o.Orders)
            .HasForeignKey(o => o.UserId)
            .IsRequired();
        
        // Each order has many order items
        builder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);
        
        // Each order item has assigned product
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .IsRequired();
        
        // Seed products table
        builder.Entity<Product>().HasData(
            new Product()
            {
                Id = 1,
                Name = "Colombian Dark Roast",
                Description = "A bold and rich coffee with notes of chocolate and caramel. Sourced from the high mountains of Colombia, this dark roast is perfect for those who love a robust and intense flavor profile.",
                Image = "https://globalassets.starbucks.com/digitalassets/products/bev/SBX20190617_FeaturedDarkRoast.jpg?impolicy=1by1_tight_288",
                Price = (decimal) 40.00
            },
            new Product()
            {
                Id = 2,
                Name = "Chai Tea Latte",
                Description = "Experience the perfect blend of bold black tea, aromatic spices, and creamy steamed milk with our Chai Tea Latte",
                Image = "https://globalassets.starbucks.com/digitalassets/products/bev/SBX20220411_ChaiLatte.jpg?impolicy=1by1_tight_288",
                Price = (decimal) 21.00
            },
            new Product()
            {
                Id = 3,
                Name = "Espresso Con Panna",
                Description = "Savor the intensity of espresso topped with a luscious dollop of whipped cream in our Espresso Con Panna, a rich and indulgent coffee delight.",
                Image = "https://globalassets.starbucks.com/digitalassets/products/bev/SBX20190617_EspressoConPanna.jpg?impolicy=1by1_tight_288",
                Price = (decimal) 20.00
            }
        );
    }
}