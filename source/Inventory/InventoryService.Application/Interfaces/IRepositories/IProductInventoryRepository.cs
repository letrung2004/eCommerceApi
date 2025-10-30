using InventoryService.Domain.Entities;
using SharedLibrarySolution.Interfaces;

namespace InventoryService.Application.Interfaces.IRepositories
{
    public interface IProductInventoryRepository : IGenericInterface<ProductsInventory>
    {
        // lấy tồn kho của biến thể sản phẩm
        Task<ProductsInventory?> GetBySkuAsync(string sku);
        Task<ProductsInventory?> GetByProductIdAsync(string productId);
        Task<bool> AdjustStockAsync(string sku, int quantityChange);
    }
}
