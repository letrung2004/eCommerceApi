using OrderSaga.Worker.DTOs;

namespace OrderSaga.Worker.Services.Interfaces
{
    public interface IPaymentServiceClient
    {
        // Payment method
        Task<(bool IsSuccessful, string PaymentId, string Message)> CreatePaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default);
        Task<bool> UpdatePaymentStatusAsync(string paymentId, string status, CancellationToken cancellationToken = default);
        Task<bool> ProcessPaymentAsync(string paymentId, CancellationToken cancellationToken = default);
        Task<bool> MarkPaymentFailedAsync(string paymentId, CancellationToken cancellationToken = default);
        Task<(bool IsSuccessful, string Message)> GetPaymentStatusAsync(string paymentId, CancellationToken cancellationToken = default);

        // Refund method
        Task<RefundDto> RequestRefundAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default);
        Task<RefundDto> ApproveRefundAsync(string refundId, CancellationToken cancellationToken = default);
        Task<RefundDto> RejectRefundAsync(string refundId, CancellationToken cancellationToken = default);
        Task<RefundDto> GetRefundByIdAsync(string refundId, CancellationToken cancellationToken = default);
    }
}
