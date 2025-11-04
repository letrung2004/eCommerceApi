using AutoMapper;
using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces.IRepositories;
using InventoryService.Domain.Enums;
using MediatR;
using SharedLibrarySolution.Exceptions;

// giai phóng hàng đã giữ quá thời hạn
namespace InventoryService.Application.Features.Reservations.Commands.ReleaseReservation
{
    public class ReleaseReservationCommandHandler : IRequestHandler<ReleaseReservationCommand, InventoryReservationDTO>
    {
        private readonly IInventoryReservationsRepository _reservationRepo;
        private readonly IProductInventoryRepository _inventoryRepo;
        private readonly IMapper _mapper;

        public ReleaseReservationCommandHandler(
            IInventoryReservationsRepository reservationRepo,
            IProductInventoryRepository inventoryRepo,
            IMapper mapper)
        {
            _reservationRepo = reservationRepo;
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
        }

        public async Task<InventoryReservationDTO> Handle(ReleaseReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _reservationRepo.GetByIdAsync(request.ReservationId);
            if (reservation == null)
                throw new AppException("Reservation not found");

            if (reservation.Status != ReservationStatus.Reserved)
                throw new AppException("Only reserved items can be released");

            // Cập nhật trạng thái và thời gian ReleasedAt
            reservation.Status = ReservationStatus.Released;
            reservation.ReleasedAt = DateTime.UtcNow;
            await _reservationRepo.UpdateAsync(reservation.Id, reservation);

            // Giảm ReservedQuantity trong tồn kho
            var inventory = await _inventoryRepo.GetBySkuAsync(reservation.Sku);
            if (inventory != null)
            {
                inventory.ReservedQuantity -= reservation.Quantity;
                inventory.UpdatedAt = DateTime.UtcNow;
                await _inventoryRepo.UpdateAsync(inventory.Id, inventory);
            }
            // event
            // TODO: Publish RabbitMQ event "inventory.released"

            return _mapper.Map<InventoryReservationDTO>(reservation);
        }
    }
}
