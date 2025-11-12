namespace OrderSaga.Worker.Services.Interfaces
{
    // gọi đến các phương thức ở inventory service
    public interface IInventoryServiceClient
    {
        /// <summary>
        /// Kiểm tra tồn kho sản phẩm.
        /// </summary>
        Task<bool> CheckStockAsync(string productId, int quantity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Đặt giữ hàng trong kho.
        /// </summary>
        Task<bool> ReserveInventoryAsync(string productId, int quantity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hủy giữ hàng trong kho.
        /// </summary>
        Task ReleaseInventoryAsync(string productId, int quantity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Xác nhận giữ hàng sau khi thanh toán thành công.
        /// </summary>
        Task ConfirmInventoryAsync(string productId, CancellationToken cancellationToken = default);
    }
}
