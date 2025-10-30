using System.Linq.Expressions;

namespace SharedLibrarySolution.Interfaces
{
    public interface IGenericInterface<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(Guid id, T entity);
        Task DeleteAsync(Guid id);
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null);
        //Task<T?> GetByAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query();
    }
}
