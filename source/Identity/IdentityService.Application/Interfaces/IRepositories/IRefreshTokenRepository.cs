using IdentityService.Domain.Entities;
using SharedLibrarySolution.Interfaces;

namespace IdentityService.Application.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken> CreateRefreshToken(RefreshToken refreshToken);
        Task InvalidateAsync(RefreshToken refreshToken);
        Task RevokeAllUserTokensAsync(Guid userId);
        Task DeleteExpiredTokensAsync();
        Task<RefreshToken?> GetByUserIdAsync(Guid userId);


    }
}
