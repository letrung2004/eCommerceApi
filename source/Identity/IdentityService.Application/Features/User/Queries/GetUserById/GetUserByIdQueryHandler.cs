using AutoMapper;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces.IRepositories;
using MediatR;

namespace IdentityService.Application.Features.User.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;           
            _mapper = mapper;
        }

        public async Task<UserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            var userDto = _mapper.Map<UserDTO>(user);
            return userDto;

        }
    }
}
