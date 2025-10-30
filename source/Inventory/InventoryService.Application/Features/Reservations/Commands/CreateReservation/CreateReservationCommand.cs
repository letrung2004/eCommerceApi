using InventoryService.Application.DTOs;
using MediatR;

namespace InventoryService.Application.Features.Reservations.Commands.CreateReservation
{
    public class CreateReservationCommand : IRequest<InventoryReservationDTO>
    {
        public string ProductId { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
    }
}
