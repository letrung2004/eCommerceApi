using IdentityService.Application.DTOs;
using MediatR;

namespace IdentityService.Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<UserDTO>
    {
        public Guid Id { get; set; }
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
