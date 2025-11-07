using PaymentService.gRPC.Domain.Enums;
using SharedLibrarySolution.Exceptions;

namespace PaymentService.gRPC.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public Payment(Guid orderId, decimal amount)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            Amount = amount;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        // thanh toán thành công
        public void MarkProcessed()
        {
            if (Status != PaymentStatus.Pending)
                throw new AppException("Chỉ xử lý thanh toán khi đang ở trạng thái Pending.");

            Status = PaymentStatus.Processed;
            CompletedAt = DateTime.UtcNow;
        }

        // thanh toán thất bai
        public void MarkFailed()
        {
            if (Status != PaymentStatus.Pending)
                throw new AppException("Chỉ đánh dấu thất bại khi đang ở trạng thái Pending.");

            Status = PaymentStatus.Failed;
            CompletedAt = DateTime.UtcNow;
        }

        // hoàn tiền
        public void MarkRefunded()
        {
            if (Status != PaymentStatus.Processed)
                throw new AppException("Chỉ hoàn tiền khi thanh toán đã được xử lý.");

            Status = PaymentStatus.Refunded;
            CompletedAt = DateTime.UtcNow;
        }

        // check trạng thái trước khi chuyển
        public bool CanTransitionTo(PaymentStatus newStatus)
        {
            return (Status, newStatus) switch {

                (PaymentStatus.Pending, PaymentStatus.Processed) => true,
                (PaymentStatus.Pending, PaymentStatus.Failed) => true,
                (PaymentStatus.Processed, PaymentStatus.Refunded) => true,
                _ => false
            };
        }
    }
}
