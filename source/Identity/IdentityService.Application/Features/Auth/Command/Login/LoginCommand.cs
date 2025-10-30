using IdentityService.Application.DTOs;
using MediatR;


namespace IdentityService.Application.Features.Auth.Command.Login
{
    public class LoginCommand : IRequest<TokenResponse>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
