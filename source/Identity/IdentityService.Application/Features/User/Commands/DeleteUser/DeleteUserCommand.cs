using IdentityService.Application.DTOs;
using MediatR;

namespace IdentityService.Application.Features.User.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest
    {
        public Guid Id { get; set; }

    }
}
