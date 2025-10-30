using SharedLibrarySolution.Interfaces;

namespace IdentityService.Application.DTOs
{
    public class PermissionDTO : IMapFrom<PermissionDTO>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
