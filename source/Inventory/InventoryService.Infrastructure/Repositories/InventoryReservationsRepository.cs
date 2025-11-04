using InventoryService.Application.Interfaces.IRepositories;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
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
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var reservation = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
            if (reservation == null)
                throw new AppException("Inventory reservation not found");

            _dbSet.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InventoryReservations>> GetAllAsync(Expression<Func<InventoryReservations, bool>>? predicate = null)
        {
            return predicate != null
                ? await _dbSet.Where(predicate).ToListAsync()
                : await _dbSet.ToListAsync();
        }

        public async Task<InventoryReservations?> GetByIdAsync(Guid id)
        {
            var reservation = await _dbSet.FindAsync(id);
            if (reservation == null)
                throw new KeyNotFoundException("Inventory reservation not found");

            return reservation;
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
            return await _dbSet
                .Where(r => r.Status == ReservationStatus.Reserved && r.ExpiresAt <= now)
                .ToListAsync();
        }

        public IQueryable<InventoryReservations> Query()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<InventoryReservations> UpdateAsync(Guid id, InventoryReservations entity)
        {
            var existing = await _dbSet.FindAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Inventory reservation not found");

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<InventoryReservations> UpdateStatusAsync(Guid id, ReservationStatus newStatus)
        {
            var existing = await _dbSet.FindAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Inventory reservation not found");

            existing.Status = newStatus;
            _context.Entry(existing).Property(x => x.Status).IsModified = true;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<InventoryReservations> ReleaseAsync(Guid id)
        {
            var existing = await _dbSet.FindAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Inventory reservation not found");

            existing.Status = ReservationStatus.Released;
            existing.ReleasedAt = DateTime.UtcNow;
            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
