using API_Doodles_2._0.Models;
using API_Doodles_2._0.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace API_Doodles_2._0.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}


    public DbSet<Users> Users { get; set; }
    public DbSet<Products> Products { get; set; }
    public DbSet<Orders> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<UserProduct> UserProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Price 
        modelBuilder.Entity<Products>()
            .Property(p => p.ProductPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(i => i.UnitPrice)
            .HasPrecision(18, 2);
        
        // Relationships
        modelBuilder.Entity<Orders>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<OrderItem>()
            .HasOne(o => o.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<OrderItem>()
            .HasOne(p => p.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // UserProduct (User owns Products like badges)
        modelBuilder.Entity<UserProduct>()
            .HasKey(up => new { up.UserId, up.ProductId });

        modelBuilder.Entity<UserProduct>()
            .HasOne(up => up.User)
            .WithMany() // keep minimal, no navigation collection required
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserProduct>()
            .HasOne(up => up.Product)
            .WithMany() // keep minimal
            .HasForeignKey(up => up.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}