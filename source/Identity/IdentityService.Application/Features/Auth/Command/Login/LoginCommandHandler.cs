using IdentityService.Application.Common.Mappings;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces.IRepositories;
using IdentityService.Application.Interfaces.Security;
using MediatR;
using SharedLibrarySolution.Exceptions;

namespace IdentityService.Application.Features.Auth.Command.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;


        public LoginCommandHandler(IUserRepository userRepository,
                                    IJwtService jwtService,
                                    IPasswordHasher passwordHasher,
                                    IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<TokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
                throw new AppException("User not found !!!");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
                throw new AppException("Password not match !!!");

            await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id);

            var tokenResponse = _jwtService.GenerateToken(user);

            var refreshTokenEntity = new Domain.Entities.RefreshToken();
            refreshTokenEntity.MapFromUserAndTokenResponse(user, tokenResponse);

            await _refreshTokenRepository.CreateRefreshToken(refreshTokenEntity);

            return tokenResponse;
        }
    }
}
