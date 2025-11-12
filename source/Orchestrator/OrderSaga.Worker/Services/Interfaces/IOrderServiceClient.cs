using OrderSaga.Worker.DTOs;

namespace OrderSaga.Worker.Services.Interfaces
{
    public interface IOrderServiceClient
    {
        // Đánh dấu đơn hàng đang được xử lý
        Task<bool> MarkOrderAsProcessingAsync(Guid orderId, CancellationToken cancellationToken = default);

        // Đánh dấu đơn hàng đã được thanh toán thành công
        Task<bool> MarkOrderAsPaidAsync(Guid orderId, CancellationToken cancellationToken = default);

        // Hủy đơn hàng với lý do cụ thể
        Task<bool> MarkOrderAsCancelledAsync(Guid orderId, string reason, CancellationToken cancellationToken = default);

        // Lấy thông tin chi tiết của đơn hàng dựa trên orderId
        Task<List<OrderItemDto>> GetOrderItemByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}
