using InventoryService.Application.Interfaces.IRepositories;
using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedLibrarySolution.Exceptions;
using System.Linq.Expressions;

namespace InventoryService.Infrastructure.Repositories
{
    public class InventoryReservationsRepository : IInventoryReservationsRepository
    {
        private readonly InventoryDbContext _context;
        private readonly DbSet<InventoryReservations> _dbSet;

        public InventoryReservationsRepository(InventoryDbContext context)
        {
            _context = context;
            _dbSet = context.Set<InventoryReservations>();
        }
        public async Task<InventoryReservations> CreateAsync(InventoryReservations entity)
        {
            await _context.InventoryReservations.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var delInventory = await _context.InventoryReservations.FirstOrDefaultAsync(x => x.Id == id);
            if (delInventory == null)
            {
                throw new AppException("InventoryReservation not found!!!");
            }
            _context.InventoryReservations.Remove(delInventory);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InventoryReservations>> GetAllAsync(Expression<Func<InventoryReservations, bool>>? predicate = null)
        {
            if (predicate != null)
                return await _dbSet.Where(predicate).ToListAsync();

            return await _dbSet.ToListAsync();
        }

        public async Task<InventoryReservations?> GetByIdAsync(Guid id)
        {
            var existing = await _dbSet.FindAsync(id);
            if (existing == null) throw new KeyNotFoundException("InventoryReservations not found");
            return existing;
        }

        public async Task<IEnumerable<InventoryReservations>> GetByOrderIdAsync(Guid orderId)
        {
            return await _dbSet.Where(r => r.OrderId == orderId).ToListAsync();
        }

        public async Task<IEnumerable<InventoryReservations>> GetByProductIdAsync(string productId)
        {
            return await _dbSet.Where(r => r.ProductId == productId).ToListAsync();
        }

        public async Task<IEnumerable<InventoryReservations>> GetExpiredReservationsAsync(DateTime now)
        {
            return await _dbSet.Where(r => r.Status == "Reserved" && r.ExpiresAt <= now).ToListAsync();
        }

        public IQueryable<InventoryReservations> Query()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<InventoryReservations> UpdateAsync(Guid id, InventoryReservations entity)
        {
            var existing = await _dbSet.FindAsync(id);
            if (existing == null) throw new KeyNotFoundException("InventoryReservations not found");

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<InventoryReservations> UpdateStatusAsync(Guid id, string newStatus)
        {
            var existing = await _dbSet.FindAsync(id);
            if (existing == null) throw new KeyNotFoundException("InventoryReservations not found");

            existing.Status = newStatus;
            _context.Entry(existing).Property(x=>x.Status).IsModified = true;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
