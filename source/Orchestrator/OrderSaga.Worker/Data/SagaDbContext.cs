using Microsoft.EntityFrameworkCore;
using OrderSaga.Worker.Entities;


namespace OrderSaga.Worker.Data
{
    public class SagaDbContext : DbContext
    {
        public SagaDbContext(DbContextOptions<SagaDbContext> options) : base(options)
        {

        }
        public DbSet<OrderSagaState> OrderSagaStates { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<OrderSagaState>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.OrderId).IsRequired();
                entity.Property(e => e.UserId)
                 .IsRequired();

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired();

                entity.Property(e => e.CurrentStep)
                    .IsRequired();

                entity.Property(e => e.StartedAt)
                    .IsRequired();

                entity.Property(e => e.FailureReason)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(e => e.PaymentTransactionId)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(e => e.CompletedAt)
                    .IsRequired(false);

                entity.Property<DateTime?>("UpdatedAt")
                    .IsRequired(false);
            });
        }
    }
}
