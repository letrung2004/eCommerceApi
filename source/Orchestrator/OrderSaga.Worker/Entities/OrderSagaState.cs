using OrderSaga.Worker.DTOs;
using OrderSaga.Worker.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderSaga.Worker.Entities
{
    public class OrderSagaState
    {
        // Định danh duy nhất của trạng thái Saga, tự động tạo khi khởi tạo
        public Guid Id { get; set; } = Guid.NewGuid();

        // Mã đơn hàng liên quan đến Saga này
        public Guid OrderId { get; set; }

        // Mã người dùng thực hiện đơn hàng
        public Guid UserId { get; set; }

        // Tổng số tiền của đơn hàng
        public decimal TotalAmount { get; set; }

        // Trạng thái hiện tại của Saga
        public SagaStatus Status { get; set; }

        // Lý do thất bại của Saga (nếu có)
        public string FailureReason { get; set; } = string.Empty;

        // Bước hiện tại của Saga trong quá trình xử lý
        public OrderSagaStep CurrentStep { get; set; }

        // Mã giao dịch thanh toán liên quan đến đơn hàng (nếu có)
        public string? PaymentTransactionId { get; set; }

        // Thời điểm Saga bắt đầu
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        // Thời điểm Saga hoàn thành (nếu đã hoàn thành)
        public DateTime? CompletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public List<OrderItemDto> ReservedItems { get; set; } = new();
    }
}
