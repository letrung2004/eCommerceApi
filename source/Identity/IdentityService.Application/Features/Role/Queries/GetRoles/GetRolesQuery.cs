using IdentityService.Application.DTOs;
using MediatR;

namespace IdentityService.Application.Features.Role.Queries.GetRoles
{
    public class GetRolesQuery : IRequest<List<RoleDTO>>
    {
    }
}
