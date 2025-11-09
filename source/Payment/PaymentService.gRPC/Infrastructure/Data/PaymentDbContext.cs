using Microsoft.EntityFrameworkCore;
using PaymentService.gRPC.Domain.Entities;

namespace PaymentService.gRPC.Infrastructure.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Refund> Refunds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ====== Cấu hình bảng Payment ======
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(p => p.Status)
                      .HasConversion<string>() // Lưu enum thành string
                      .IsRequired();

                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // ====== Cấu hình bảng Refund ======
            modelBuilder.Entity<Refund>(entity =>
            {
                entity.ToTable("Refunds");

                entity.HasKey(r => r.Id);

                entity.Property(r => r.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(r => r.Status)
                      .HasConversion<string>() // Lưu enum RefundStatus dạng string
                      .IsRequired();

                entity.Property(r => r.RequestedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(r => r.OrderId)
                      .IsRequired();

                // (Tuỳ chọn) tạo chỉ mục để truy vấn theo OrderId nhanh hơn
                entity.HasIndex(r => r.OrderId);
            });
        }
    }
}
