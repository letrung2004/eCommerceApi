using SharedLibrarySolution.Interfaces;

namespace OrderService.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericInterface<T> GetRepository<T>() where T : class;
        Task CommitAsync();
    }
}
