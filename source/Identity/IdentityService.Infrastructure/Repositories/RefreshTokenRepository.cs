using IdentityService.Application.Interfaces.IRepositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;



namespace IdentityService.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IdentityDbContext _context;

        public RefreshTokenRepository(IdentityDbContext context)
        {
            _context = context;
        }
        public async Task<RefreshToken> CreateRefreshToken(RefreshToken refreshToken)
        {
            await _context.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task DeleteExpiredTokensAsync()
        {
            var expiredTokens = await _context.RefreshToken.Where(x => x.ExpiresAt < DateTime.UtcNow && !x.IsRevoked).ToListAsync();
            _context.RefreshToken.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshToken.FirstOrDefaultAsync(x => x.Token == token);
        }

        //lấy refresh token của user trước khi đăng xuất
        public async Task<RefreshToken?> GetByUserIdAsync(Guid userId)
        {
            var token = await _context.RefreshToken
                    .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsRevoked);
            return token;
        }

        public async Task InvalidateAsync(RefreshToken refreshToken)
        {
            refreshToken.IsRevoked = true;
            _context.RefreshToken.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllUserTokensAsync(Guid userId)
        {
            var userTokens = await _context.RefreshToken.Where(x => x.UserId == userId && !x.IsRevoked).ToListAsync();
            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
