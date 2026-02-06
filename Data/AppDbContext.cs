using Microsoft.EntityFrameworkCore;
using OrdersBackend.Models;

namespace OrdersBackend.Data;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("ThePirahns"); 

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}