using IdentityService.Application.DTOs;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Common.Mappings
{
    public static class RefreshTokenMapper
    {
        public static void MapFromUserAndTokenResponse(
            this RefreshToken entity,
            User user,
            TokenResponse tokenResponse)
        {
            entity.UserId = user.Id;
            entity.Token = tokenResponse.RefreshToken;
            entity.ExpiresAt = tokenResponse.RefreshTokenExpiration;
            entity.IsRevoked = false;
        }
    }
}
