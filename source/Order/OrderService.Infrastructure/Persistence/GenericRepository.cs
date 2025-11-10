using Microsoft.EntityFrameworkCore;
using SharedLibrarySolution.Interfaces;
using System.Linq.Expressions;

namespace OrderService.Infrastructure.Persistence
{
    public class GenericRepository<T> : IGenericInterface<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Thêm entity vào DbSet, chưa commit
        public async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        // Update entity, chưa commit
        public async Task<T> UpdateAsync(Guid id, T entity)
        {
            var existing = await _dbSet.FindAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Entity với Id {id} không tồn tại");

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        // Remove entity, chưa commit
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Entity với Id {id} không tồn tại");

            _dbSet.Remove(entity);
        }

        // Lấy entity theo Id
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Lấy tất cả hoặc theo điều kiện
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate != null)
                return await _dbSet.Where(predicate).ToListAsync();

            return await _dbSet.ToListAsync();
        }

        // Trả IQueryable để query thêm, filter, include...
        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }
    }
}
