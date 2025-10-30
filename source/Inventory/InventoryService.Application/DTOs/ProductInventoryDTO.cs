using InventoryService.Domain.Entities;
using SharedLibrarySolution.Interfaces;

namespace InventoryService.Application.DTOs
{
    public class ProductInventoryDTO : IMapFrom<ProductsInventory>
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
