using IdentityService.Application.Interfaces.IRepositories;
using MediatR;
using SharedLibrarySolution.Exceptions;

namespace IdentityService.Application.Features.Auth.Command.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GetByUserIdAsync(request.Id);
            if (token == null)
            {
                throw new AppException("Refresh token invalid");
            }
            await _refreshTokenRepository.InvalidateAsync(token);

            return true;
        }
    }
}
