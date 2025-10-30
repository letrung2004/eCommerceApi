using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options) { }

        public DbSet<InventoryEvent> InventoryEvents { get; set; } = null!;
        public DbSet<InventoryReservations> InventoryReservations { get; set; } = null!;
        public DbSet<ProductsInventory> ProductsInventory { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductsInventory>()
                .HasIndex(p => new { p.ProductId, p.Sku })
                .IsUnique();

            modelBuilder.Entity<InventoryReservations>()
                .HasIndex(r => new { r.OrderId, r.Sku });
        }

    }
}
