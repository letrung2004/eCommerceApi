using IdentityService.Application.DTOs;
using MediatR;

namespace IdentityService.Application.Features.User.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserDTO>
    {
        public Guid Id { get; set; }
    }
}
