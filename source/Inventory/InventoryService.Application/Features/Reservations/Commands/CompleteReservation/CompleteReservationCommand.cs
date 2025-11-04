using InventoryService.Application.DTOs;
using MediatR;

namespace InventoryService.Application.Features.Reservations.Commands.CompleteReservation
{
    public class CompleteReservationCommand : IRequest<InventoryReservationDTO>
    {
        public Guid ReservationId { get; set; }
    }

}
