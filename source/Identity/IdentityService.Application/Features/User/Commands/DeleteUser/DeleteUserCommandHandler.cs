using IdentityService.Application.Interfaces.IRepositories;
using MediatR;
using SharedLibrarySolution.Exceptions;

namespace IdentityService.Application.Features.User.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;
        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
       

        async Task IRequestHandler<DeleteUserCommand>.Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new AppException("User not found");
            }
            await _userRepository.DeleteAsync(user!.Id);
        }
    }
}
