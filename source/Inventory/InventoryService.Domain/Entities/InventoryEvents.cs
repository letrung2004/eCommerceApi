namespace InventoryService.Domain.Entities
{
    public class InventoryEvent
    {
        public Guid Id { get; set; }

        // Loại sự kiện: inventory.reserved, inventory.released, inventory.updated,...
        public string EventType { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        // Số lượng thay đổi (vd: +10 khi nhập hàng, -2 khi giữ hàng,...)
        public int Quantity { get; set; }
        public string? Data { get; set; }
        // Thời điểm phát sinh event
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
