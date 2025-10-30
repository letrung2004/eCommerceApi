using InventoryService.Application.DTOs;
using MediatR;

namespace InventoryService.Application.Features.Reservations.Queries.GetReservationById
{
    public class GetReservationsByOrderIdQuery : IRequest<IEnumerable<InventoryReservationDTO>>
    {
        public Guid OrderId { get; set; }
    }
}
