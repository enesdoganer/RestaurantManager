using Microsoft.EntityFrameworkCore;
using RestaurantManager.Core.Models;
using RestaurantManager.Core.Services;

namespace RestaurantManager.Core.Data;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Table> Tables { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public DbContext(DbContextOptions<DbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Table
        modelBuilder.Entity<Table>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Id).ValueGeneratedNever();
            e.Property(t => t.Seats);
        });

        // MenuItem
        modelBuilder.Entity<MenuItem>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Id).ValueGeneratedOnAdd();
            e.Property(m => m.Price).HasColumnType("numeric(10,2)");
            e.Property(m => m.Ingredients)
                .HasConversion(
                    i => i == null ? null : string.Join(',', i),
                    i => i == null
                        ? new List<string>() 
                        : i.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList());
            e.Property(m => m.Allergens)
                .HasConversion(
                    v => string.Join(',', v.Select(a => a.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => Enum.Parse<Allergen>(a))
                        .ToList());
        });

        // Order
        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.Ignore(o => o.TotalPrice);
            e.Property(o => o.CreatedAt)
                .HasConversion<DateTime>(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            e.HasOne<Table>()
                .WithMany()
                .HasForeignKey(o => o.TableId);
            e.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasKey(oi => oi.Id);
            e.Property(oi => oi.Id).ValueGeneratedOnAdd();
            e.HasOne(oi => oi.MenuItem)
                .WithMany()
                .HasForeignKey(oi => oi.MenuItemId);
        });
    }
}

public static class QueryExtensions 
{
    public static IQueryable<MenuItem> AsSorted(this IQueryable<MenuItem> query) 
        => query.OrderBy(t => t.Id);
    public static IQueryable<Table> AsSorted(this IQueryable<Table> query) 
        => query.OrderBy(t => t.Id);
    public static IQueryable<Order> AsSorted(this IQueryable<Order> query) 
        => query.OrderBy(t => t.Id);
}