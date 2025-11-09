using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using SharedLibrarySolution.Interfaces;

namespace OrderService.Application.DTOs
{
    public class OrderResponse : IMapFrom<Order>
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public decimal TotalPrice { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime ?UpdatedAt { get; private set; }
        public ICollection<OrderItemResponse> OrderItems { get; private set; } = new List<OrderItemResponse>();
        public bool IsRefunded { get; private set; } // Đánh dấu đơn hàng có hoàn tiền không

    }
}
// note here 07/11/2025