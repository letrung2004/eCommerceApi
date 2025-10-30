using InventoryService.Application.Interfaces.IRepositories;
using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryService.Infrastructure.Repositories
{
    public class ProductInventoryRepository : IProductInventoryRepository
    {
        private readonly InventoryDbContext _context;
        private readonly DbSet<ProductsInventory> _dbSet;

        public ProductInventoryRepository(InventoryDbContext context)
        {
            _context = context;
            _dbSet = context.Set<ProductsInventory>();
        }

        // --- Implement IGenericInterface ---
        public async Task<ProductsInventory> CreateAsync(ProductsInventory entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductsInventory>> GetAllAsync(Expression<Func<ProductsInventory, bool>>? predicate = null)
        {
            if (predicate != null)
                return await _dbSet.Where(predicate).ToListAsync();

            return await _dbSet.ToListAsync();
        }

        public async Task<ProductsInventory?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<ProductsInventory> UpdateAsync(Guid id, ProductsInventory entity)
        {
            var existing = await _dbSet.FindAsync(id);
            if (existing == null) throw new KeyNotFoundException("Inventory not found");

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public IQueryable<ProductsInventory> Query()
        {
            return _dbSet.AsQueryable();
        }

        // --- Implement phần đặc thù ---
        public async Task<ProductsInventory?> GetBySkuAsync(string sku)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Sku == sku);
        }

        public async Task<ProductsInventory?> GetByProductIdAsync(string productId)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == productId);
        }

        public async Task<bool> AdjustStockAsync(string sku, int quantityChange)
        {
            var inventory = await _dbSet.FirstOrDefaultAsync(x => x.Sku == sku);
            if (inventory == null) return false;

            inventory.StockQuantity += quantityChange;
            inventory.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
