using Microsoft.EntityFrameworkCore;
using EnigmaStock.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace EnigmaStock.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
  
    public DbSet<Category> Categories { get; set; } 
    public DbSet<Product>Products { get; set; }
    public DbSet<StockMovement> StockMovement { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchaseItem> purchaseItems { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

    }
}
