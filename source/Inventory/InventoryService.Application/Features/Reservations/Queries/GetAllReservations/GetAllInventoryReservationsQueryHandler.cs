using AutoMapper;
using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces.IRepositories;
using MediatR;

namespace InventoryService.Application.Features.Reservations.Queries.GetAllReservations
{
    public class GetAllInventoryReservationsQueryHandler : IRequestHandler<GetAllInventoryReservationsQuery, IEnumerable<InventoryReservationDTO>>
    {
        private readonly IInventoryReservationsRepository _repo;
        private readonly IMapper _mapper;

        public GetAllInventoryReservationsQueryHandler(IInventoryReservationsRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<IEnumerable<InventoryReservationDTO>> Handle(GetAllInventoryReservationsQuery request, CancellationToken cancellationToken)
        {
            var reservations = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<InventoryReservationDTO>>(reservations);
        }
    }
}
