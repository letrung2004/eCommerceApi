using InventoryService.Domain.Entities;
using SharedLibrarySolution.Interfaces;

namespace InventoryService.Application.Interfaces.IRepositories
{
    public interface IInventoryReservationsRepository : IGenericInterface<InventoryReservations>
    {
        /// <summary>
        /// Lấy tất cả reservation của một order
        /// </summary>
        Task<IEnumerable<InventoryReservations>> GetByOrderIdAsync(Guid orderId);

        /// <summary>
        /// Lấy tất cả reservation của một sản phẩm
        /// </summary>
        Task<IEnumerable<InventoryReservations>> GetByProductIdAsync(string productId);

        /// <summary>
        /// Lấy các reservation hết hạn để auto-release
        /// </summary>
        Task<IEnumerable<InventoryReservations>> GetExpiredReservationsAsync(DateTime now);

        /// <summary>
        /// Chỉ đổi trạng thái reservation (Reserved, Released, Completed)
        /// </summary>
        Task<InventoryReservations> UpdateStatusAsync(Guid id, string newStatus);
    }
}
