using InventoryService.Application.DTOs;
using MediatR;

namespace InventoryService.Application.Features.Reservations.Queries.GetAllReservations
{
    public class GetAllInventoryReservationsQuery : IRequest<IEnumerable<InventoryReservationDTO>>
    {
    }
}
