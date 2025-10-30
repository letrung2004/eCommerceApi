using IdentityService.Application.DTOs;
using MediatR;

namespace IdentityService.Application.Features.User.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<UserDTO>
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
