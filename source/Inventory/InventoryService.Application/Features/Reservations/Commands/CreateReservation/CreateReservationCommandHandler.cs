using AutoMapper;
using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces.IRepositories;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using MediatR;
using SharedLibrarySolution.Exceptions;

namespace InventoryService.Application.Features.Reservations.Commands.CreateReservation
{
    public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, InventoryReservationDTO>
    {
        private readonly IInventoryReservationsRepository _reservationRepo;
        private readonly IProductInventoryRepository _inventoryRepo;
        private readonly IMapper _mapper;

        public CreateReservationCommandHandler(
            IInventoryReservationsRepository reservationRepo,
            IProductInventoryRepository inventoryRepo,
            IMapper mapper)
        {
            _reservationRepo = reservationRepo;
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
        }

        public async Task<InventoryReservationDTO> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepo.GetBySkuAsync(request.Sku);
            if (inventory == null)
                throw new AppException("Product inventory not found");

            var available = inventory.StockQuantity - inventory.ReservedQuantity;
            if (available < request.Quantity)
                throw new AppException("Not enough stock to reserve");

            // Tăng số lượng giữ hàng
            inventory.ReservedQuantity += request.Quantity;
            inventory.UpdatedAt = DateTime.UtcNow;
            await _inventoryRepo.UpdateAsync(inventory.Id, inventory);

            // Tạo bản ghi giữ hàng
            var reservation = new InventoryReservations
            {
                Id = Guid.NewGuid(),
                OrderId = request.OrderId,
                ProductId = request.ProductId,
                Sku = request.Sku,
                Quantity = request.Quantity,
                Status = ReservationStatus.Reserved,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };

            await _reservationRepo.CreateAsync(reservation);

            // TODO: Publish RabbitMQ event "inventory.reserved"

            return _mapper.Map<InventoryReservationDTO>(reservation);
        }
    }
}
