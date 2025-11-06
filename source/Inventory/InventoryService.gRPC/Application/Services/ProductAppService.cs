using InventoryService.gRPC.Domain.Entities;
using InventoryService.gRPC.Domain.IRepositories;

// điều phối các nghiệp vụ của product service gửi qua 
// -> thêm sản phẩm mới -> cập nhật lại kho
// chịu trách nhiệm tạo/cập nhật tồn kho sản phẩm (khi có sản phẩm mới).

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
            return true;
        }
    }
}

