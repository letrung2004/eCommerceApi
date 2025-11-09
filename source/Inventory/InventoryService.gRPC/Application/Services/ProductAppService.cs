using InventoryService.gRPC.Domain.Entities;
using InventoryService.gRPC.Domain.IRepositories;

namespace InventoryService.gRPC.Application.Services
{
    public class ProductAppService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public ProductAppService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<bool> CreateInventory(string productId, int qty)
        {
            var product = await _inventoryRepository.GetByProductIdAsync(productId);
            if (product == null)
            {
                var newProduct = new InventoryItem(productId, qty);
                await _inventoryRepository.AddAsync(newProduct);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateQuantityInventory(string productId, int qty)
        {
            var product = await _inventoryRepository.GetByProductIdAsync(productId);
            if (product == null) return false;

            product.UpdateQuantity(qty);
            await _inventoryRepository.UpdateAsync(product);

            return true;
        }
    }
}
