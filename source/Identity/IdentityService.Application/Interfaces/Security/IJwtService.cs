using IdentityService.Application.DTOs;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces.Security
{
    public interface IJwtService
    {
        TokenResponse GenerateToken(User user);
    }
}
