using AutoMapper;
using IdentityService.Application.Common.Mappings;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces.IRepositories;
using IdentityService.Application.Interfaces.Security;
using MediatR;
using SharedLibrarySolution.Exceptions;

namespace IdentityService.Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMediator _mediator;
        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper, IPasswordHasher passwordHasher, IMediator mediator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _mediator = mediator;
        }
        public async Task<UserDTO> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
                throw new AppException("User not found");

            user.MapFromUpdateCommand(request);

            //băm lại nếu có password mới
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            }
            await _userRepository.UpdateAsync(user.Id, user);

            return _mapper.Map<UserDTO>(user);
        }

    }
    
}
