using IdentityService.Domain.Entities;
using SharedLibrarySolution.Interfaces;

namespace IdentityService.Application.DTOs
{
    public class RoleDTO : IMapFrom<Role>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<PermissionDTO>? Permissions { get; set; }
    }
}
