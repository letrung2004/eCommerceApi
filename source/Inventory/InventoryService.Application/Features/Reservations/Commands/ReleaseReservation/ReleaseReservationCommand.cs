using InventoryService.Application.DTOs;
using MediatR;

namespace InventoryService.Application.Features.Reservations.Commands.ReleaseReservation
{
    public class ReleaseReservationCommand : IRequest<InventoryReservationDTO>
    {
        public Guid ReservationId { get; set; }
    }
}
