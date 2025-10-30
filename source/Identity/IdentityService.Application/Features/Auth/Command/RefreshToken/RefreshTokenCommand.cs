using IdentityService.Application.DTOs;
using MediatR;


namespace IdentityService.Application.Features.Auth.Command.RefreshToken
{
    public class RefreshTokenCommand : IRequest<TokenResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
