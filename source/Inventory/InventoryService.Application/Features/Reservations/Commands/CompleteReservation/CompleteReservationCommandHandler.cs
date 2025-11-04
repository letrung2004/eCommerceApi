using AutoMapper;
using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces.IRepositories;
using InventoryService.Domain.Enums;
using MediatR;
using SharedLibrarySolution.Exceptions;

namespace InventoryService.Application.Features.Reservations.Commands.CompleteReservation
{
    public class CompleteReservationCommandHandler : IRequestHandler<CompleteReservationCommand, InventoryReservationDTO>
    {
        private readonly IInventoryReservationsRepository _reservationRepo;
        private readonly IProductInventoryRepository _inventoryRepo;
        private readonly IMapper _mapper;

        public CompleteReservationCommandHandler(
            IInventoryReservationsRepository reservationRepo,
            IProductInventoryRepository inventoryRepo,
            IMapper mapper)
        {
            _reservationRepo = reservationRepo;
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
        }

        public async Task<InventoryReservationDTO> Handle(CompleteReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _reservationRepo.GetByIdAsync(request.ReservationId);
            if (reservation == null)
                throw new AppException("Reservation not found");

            if (reservation.Status != ReservationStatus.Reserved)
                throw new AppException("Only reserved items can be completed");

            var inventory = await _inventoryRepo.GetBySkuAsync(reservation.Sku);
            if (inventory == null)
                throw new AppException("Product inventory not found");

            if (inventory.StockQuantity < reservation.Quantity)
                throw new AppException("Not enough stock to complete reservation");

            // Trừ kho chính thức
            inventory.StockQuantity -= reservation.Quantity;
            inventory.ReservedQuantity -= reservation.Quantity;
            inventory.UpdatedAt = DateTime.UtcNow;
            await _inventoryRepo.UpdateAsync(inventory.Id, inventory);

            // Cập nhật trạng thái giữ hàng
            reservation.Status = ReservationStatus.Completed;
            await _reservationRepo.UpdateAsync(reservation.Id, reservation);

            // TODO: Publish event "inventory.completed"

            return _mapper.Map<InventoryReservationDTO>(reservation);
        }
    }

}
