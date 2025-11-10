using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public decimal TotalPrice { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
        public bool IsRefunded { get; private set; } // Đánh dấu đơn hàng có hoàn tiền không

        private Order() { }

        public Order(Guid id, Guid userId, IEnumerable<OrderItem> items)
        {
            Id = id;
            UserId = userId;
            OrderItems = items.ToList();
            TotalPrice = CalculateTotal();
            Status = OrderStatus.Pending;
        }

        // Tính tổng đơn hàng dựa trên OrderItems hiện tại
        public decimal CalculateTotal()
        {
            return OrderItems.Sum(item => item.TotalPrice);
        }

        // Thêm nhiều OrderItem vào đơn
        public void AddOrderItems(IEnumerable<OrderItem> items)
        {
            foreach (var item in items)
            {
                if (item.OrderId != this.Id)
                {
                    throw new InvalidOperationException($"OrderItem có OrderId {item.OrderId} không khớp với Order {this.Id}");
                }

                OrderItems.Add(item);
            }

            // Cập nhật lại tổng giá trị đơn
            TotalPrice = CalculateTotal();
        }

        // ======== Các phương thức trạng thái đơn hàng ========
        public void MarkAsProcessing()
        {
            Status = OrderStatus.PaymentProcessing;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsPaid()
        {
            Status = OrderStatus.PaymentCompleted;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsShipped()
        {
            Status = OrderStatus.Shipped;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsCancelled()
        {
            Status = OrderStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        // ======== Hoàn tiền ========
        public void RequestRefund()
        {
            if (Status != OrderStatus.Shipped && Status != OrderStatus.Delivered)
            {
                throw new InvalidOperationException("Chỉ có thể yêu cầu hoàn tiền nếu đơn hàng đã được giao");
            }

            IsRefunded = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void CompleteRefund()
        {
            if (!IsRefunded)
            {
                throw new InvalidOperationException("Không thể hoàn tiền nếu đơn hàng chưa có yêu cầu hoàn trả");
            }

            Status = OrderStatus.Refunded;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
