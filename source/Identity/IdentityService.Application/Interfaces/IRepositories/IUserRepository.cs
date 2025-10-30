using IdentityService.Domain.Entities;
using SharedLibrarySolution.Interfaces;

namespace IdentityService.Application.Interfaces.IRepositories
{
    public interface IUserRepository : IGenericInterface<User>
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}
