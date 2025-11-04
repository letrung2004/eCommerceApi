using InventoryService.Application.Interfaces.IRepositories;
using InventoryService.Domain.Enums;
using Quartz;

namespace InventoryService.Application.Jobs
{
    public class ExpiredReservationCleanupJob : IJob
    {
        private readonly IInventoryReservationsRepository _reservationRepo;
        private readonly IProductInventoryRepository _inventoryRepo;

        public ExpiredReservationCleanupJob(
            IInventoryReservationsRepository reservationRepo,
            IProductInventoryRepository inventoryRepo)
        {
            _reservationRepo = reservationRepo;
            _inventoryRepo = inventoryRepo;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var expired = await _reservationRepo.GetExpiredReservationsAsync(DateTime.UtcNow);  // lấy các resver hết hạn
            foreach (var reservation in expired)
            {
                reservation.Status = ReservationStatus.Expired;
                reservation.ReleasedAt = DateTime.UtcNow;

                await _reservationRepo.UpdateAsync(reservation.Id, reservation); // cập nhật ở inventory reservation
                // lấy ở Inventory product - cập nhật là reserver quanlity và thời gian cập nhật
                var inventory = await _inventoryRepo.GetBySkuAsync(reservation.Sku);
                if (inventory != null) {

                    inventory.ReservedQuantity -= reservation.Quantity;
                    inventory.UpdatedAt = DateTime.UtcNow;
                    await _inventoryRepo.UpdateAsync(inventory.Id, inventory);
                }

            }
        }
    }
}
