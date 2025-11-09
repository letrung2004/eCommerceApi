using InventoryService.gRPC.Domain.Entities;

namespace InventoryService.gRPC.Domain.IRepositories
{
    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetByProductIdAsync(string productId);
        Task AddAsync(InventoryItem item);
        Task UpdateAsync(InventoryItem item);
    }
}
