using MediatR;


namespace IdentityService.Application.Features.Auth.Command.Logout
{
    public class LogoutCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
