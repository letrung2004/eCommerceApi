using IdentityService.Application.DTOs;
using MediatR;

namespace IdentityService.Application.Features.Role.Queries.GetRoles
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDTO>>
    {
        public Task<List<RoleDTO>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
