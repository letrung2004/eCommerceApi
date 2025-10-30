using AutoMapper;
using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces.IRepositories;
using MediatR;

namespace InventoryService.Application.Features.Reservations.Queries.GetReservationById
{
    public class GetReservationsByOrderIdQueryHandler : IRequestHandler<GetReservationsByOrderIdQuery, IEnumerable<InventoryReservationDTO>>
    {
        private readonly IInventoryReservationsRepository _repo;
        private readonly IMapper _mapper;

        public GetReservationsByOrderIdQueryHandler(IInventoryReservationsRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InventoryReservationDTO>> Handle(GetReservationsByOrderIdQuery request, CancellationToken cancellationToken)
        {
            var reservations = await _repo.GetByOrderIdAsync(request.OrderId);
            return _mapper.Map<IEnumerable<InventoryReservationDTO>>(reservations);
        }
    }
}
