using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        // Các DbSet
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Cấu hình Order =====
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                entity.HasKey(o => o.Id);

                entity.Property(o => o.UserId)
                      .IsRequired();

                entity.Property(o => o.TotalPrice)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(o => o.Status)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(o => o.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(o => o.UpdatedAt)
                      .IsRequired(false);

                entity.Property(o => o.IsRefunded)
                      .IsRequired();
            });

            // ===== Cấu hình OrderItem =====
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");

                entity.HasKey(oi => oi.Id);

                entity.Property(oi => oi.ProductId)
                      .IsRequired();

                entity.Property(oi => oi.Quantity)
                      .IsRequired();

                entity.Property(oi => oi.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                // Tính tổng có thể dùng TotalPrice nhưng không cần lưu vào DB
                entity.Ignore(oi => oi.TotalPrice);

                // Quan hệ 1 Order - N OrderItem
                entity.HasOne<Order>()
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Index để tìm nhanh theo OrderId
                entity.HasIndex(oi => oi.OrderId);
            });
        }
    }
}
