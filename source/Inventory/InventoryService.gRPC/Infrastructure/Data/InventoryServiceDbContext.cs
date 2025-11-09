using InventoryService.gRPC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.gRPC.Infrastructure.Data
{
    public class InventoryServiceDbContext : DbContext
    {
        public InventoryServiceDbContext(DbContextOptions<InventoryServiceDbContext> options)
            : base(options)
        {
        }

        public DbSet<InventoryItem> InventoryItems { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình key
            modelBuilder.Entity<InventoryItem>()
                .HasKey(i => i.ProductId);

            // EF Core cần constructor không tham số
            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.ProductId)
                .IsRequired();

            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.AvailableQuantity)
                .IsRequired();

            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.ReservedQuantity)
                .IsRequired();

            // Nếu muốn, bạn có thể map private Id field
            modelBuilder.Entity<InventoryItem>()
                .Ignore(i => i.Id); // hoặc dùng HasKey với Id nếu muốn
        }
    }
}
