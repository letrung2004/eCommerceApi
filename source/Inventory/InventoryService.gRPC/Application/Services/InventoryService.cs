using InventoryService.gRPC.Domain.IRepositories;

// chịu trách nhiệm quản lý và kiểm tra tồn kho (khi khách đặt hàng, hủy đơn, xác nhận thanh toán,...).

namespace InventoryService.gRPC.Application.Services
{
    public class InventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<bool> CheckAvailability(string productId, int qty)
        {
            var item = await _inventoryRepository.GetByProductIdAsync(productId);
            return item != null && item.HasSufficientQuantity(qty);
        }

        public async Task<bool> ReserveItem(string productId, int qty)
        {
            var item = await _inventoryRepository.GetByProductIdAsync(productId);
            if (item == null || !item.HasSufficientQuantity(qty)) return false;

            item.Reserve(qty);
            await _inventoryRepository.UpdateAsync(item);
            return true;
        }

        public async Task<bool> ReleaseItem(string productId, int qty)
        {
            var item = await _inventoryRepository.GetByProductIdAsync(productId);
            if (item == null) return false;

            item.Release(qty);
            await _inventoryRepository.UpdateAsync(item);
            return true;
        }

        public async Task ConfirmItem(string productId)
        {
            var item = await _inventoryRepository.GetByProductIdAsync(productId);
            if (item == null) throw new KeyNotFoundException("Product not found");

            item.Confirm();
            await _inventoryRepository.UpdateAsync(item);
        }

    }
}

