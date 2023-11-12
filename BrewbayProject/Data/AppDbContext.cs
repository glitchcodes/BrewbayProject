using BrewbayProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BrewbayProject.Data;

public class AppDbContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
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
            .WithOne(p => p.OrderItem)
            .HasForeignKey<OrderItem>(oi => oi.ProductId)
            .IsRequired();
    }
}