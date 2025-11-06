using InventoryService.gRPC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.gRPC.Infrastructure.Data
{
    public class InventoryServiceDbContext : DbContext
    {
        public InventoryServiceDbContext(DbContextOptions<InventoryServiceDbContext> options) : base(options)
        {

        }
        public DbSet<InventoryItem> InventoryItems { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryItem>().HasKey(i => i.ProductId);
        }
    }
}
