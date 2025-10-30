using AutoMapper;
using IdentityService.Application.Common.Mappings;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces.IRepositories;
using IdentityService.Application.Interfaces.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedLibrarySolution.Exceptions;

namespace IdentityService.Application.Features.User.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMediator _mediator;
        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper, IPasswordHasher passwordHasher, IMediator mediator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _mediator = mediator;
        }
        public async Task<UserDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = new Domain.Entities.User();
                user.MapFromCreateCommand(request);
                user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash); // băm mkhau

                await _userRepository.CreateAsync(user);
                var userDto = _mapper.Map<UserDTO>(user);
                return userDto;
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? string.Empty;

                if (innerMessage.Contains("IX_User_Email"))
                    throw new AppException("Email exited");

                if (innerMessage.Contains("IX_User_Username"))
                    throw new AppException("User exited");

                throw new AppException("Data duplicate");
            }
        
    }
    }
}
