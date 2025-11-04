using InventoryService.Domain.Enums;

namespace InventoryService.Domain.Entities
{
    public class InventoryReservations
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set;} = DateTime.UtcNow;
        public DateTime ReleasedAt { get; set; }
    }
}
