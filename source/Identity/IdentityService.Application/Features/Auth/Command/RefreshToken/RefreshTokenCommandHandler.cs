using IdentityService.Application.Common.Mappings;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces.IRepositories;
using IdentityService.Application.Interfaces.Security;
using MediatR;
using SharedLibrarySolution.Exceptions;


namespace IdentityService.Application.Features.Auth.Command.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
    {
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenCommandHandler(
            IJwtService jwtService,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
                throw new AppException("Token invalid");

            var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
            if (user == null)
                throw new AppException("User not found");

            //Khi refresh token được dùng, toàn bộ session cũ của user sẽ bị đăng xuất. trên nhiều thiết bị phải đăng nhập lại(cần chỉnh)
            await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id);

            var newAccessToken = _jwtService.GenerateToken(user);
            var refreshTokenEntity = new Domain.Entities.RefreshToken();
            refreshTokenEntity.MapFromUserAndTokenResponse(user, newAccessToken);

            await _refreshTokenRepository.CreateRefreshToken(refreshTokenEntity);

            return newAccessToken;
        }
    }
}