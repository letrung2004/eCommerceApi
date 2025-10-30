namespace InventoryService.Domain.Entities
{
    public class ProductsInventory
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
    }
}
