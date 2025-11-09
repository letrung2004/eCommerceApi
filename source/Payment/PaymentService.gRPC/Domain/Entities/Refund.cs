using PaymentService.gRPC.Domain.Enums;

namespace PaymentService.gRPC.Domain.Entities
{
    public class Refund
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public decimal Amount { get; private set; } // Số tiền hoàn trả
        public RefundStatus Status { get; private set; } // Trạng thái hoàn tiền
        public DateTime RequestedAt { get; private set; } = DateTime.UtcNow; // Ngày yêu cầu hoàn tiền

        public Refund(Guid id, Guid orderId, decimal amount)
        {
            OrderId = orderId;
            Amount = amount;
            Status = RefundStatus.Requested;
        }
        public void ApproveRefund()
        {
            Status = RefundStatus.Approved;
        }

        public void RejectRefund()
        {
            Status = RefundStatus.Rejected;
        }
    }
}


//[User]
//   |
//   | 1.Gửi request refund(HTTP)
//   v
//[OrderService.API]
//   |
//   | ->gọi Application layer: order.RequestRefund()
//   | ->đổi trạng thái nội bộ: IsRefunded = true(pending)
//   | ->publish event: OrderRefundRequestedEvent(OrderId, PaymentId, Amount)
//   v
//[Message Broker / RabbitMQ]
//   |
//   | 2. Event được Orchestrator bắt
//   v
//[SagaOrchestrator]
//   |
//   | -> gọi PaymentService.RefundPaymentAsync(PaymentId, Amount)
//   | -> chờ kết quả gRPC / message
//   v
//[PaymentService]
//   |
//   | -> tạo Refund entity (RefundId, PaymentId, Amount)
//   | -> gọi cổng thanh toán thực hiện hoàn tiền
//   | -> cập nhật Refund.Status = Success hoặc Failed
//   | -> publish PaymentRefundCompletedEvent / PaymentRefundFailedEvent
//   v
//[Message Broker / RabbitMQ]
//   |
//   | 3. Orchestrator nghe PaymentRefundCompletedEvent
//   | -> gọi OrderService.UpdateOrderStatus(orderId, Refunded)
//   v
//[OrderService]
//   |
//   | -> gọi domain method: order.CompleteRefund()
//   | -> cập nhật trạng thái: Status = Refunded
//   | -> lưu vào DB
//   v
//[User]
//   |
//   | -> Nhận thông báo: “Đơn hàng #1234 đã được hoàn tiền thành công”
