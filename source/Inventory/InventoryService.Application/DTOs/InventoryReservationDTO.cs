using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using SharedLibrarySolution.Interfaces;

namespace InventoryService.Application.DTOs
{
    public class InventoryReservationDTO : IMapFrom<InventoryReservations>
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// Trạng thái của reservation:
        /// - "Reserved": đang giữ hàng
        /// - "Released": đã hủy / hết hạn
        /// - "Completed": đã dùng khi order hoàn tất
        /// </summary>
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Thời điểm hết hạn giữ hàng.
        /// Dùng cho Quartz/BackgroundService để auto release.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
        public DateTime ReleasedAt { get; set; }

    }
}
