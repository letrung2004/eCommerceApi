using InventoryService.gRPC.Domain.Entities;
using InventoryService.gRPC.Domain.IRepositories;
using InventoryService.gRPC.Infrastructure.Data;

namespace InventoryService.gRPC.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryServiceDbContext _context;

        public InventoryRepository(InventoryServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(InventoryItem item)
        {
            await _context.InventoryItems.AddAsync(item);
            await _context.SaveChangesAsync(); // commit luôn
        }

        public async Task<InventoryItem?> GetByProductIdAsync(string productId)
        {
            return await _context.InventoryItems.FindAsync(productId);
        }

        public async Task UpdateAsync(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync(); 
        }
    }
}
