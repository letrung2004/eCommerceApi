using AutoMapper;
using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces.IRepositories;
using MediatR;

namespace InventoryService.Application.Features.Reservations.Commands.CreateReservation
{
    public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, InventoryReservationDTO>
    {
        private readonly IInventoryReservationsRepository _repo;
        private readonly IMapper _mapper;
        public CreateReservationCommandHandler(IInventoryReservationsRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public Task<InventoryReservationDTO> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
