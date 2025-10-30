namespace BasketService.Presentation.Entities
{
    public class BasketItem
    {
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public string? Sku { get; set; } // null nếu không có variant
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; } // 0 nếu không có giảm giá

        // ✅ Helper properties
        public decimal FinalPrice => DiscountPrice > 0 ? DiscountPrice : Price;
        public decimal TotalPrice => FinalPrice * Quantity;

        // Key để phân biệt item trong giỏ
        public string UniqueKey => string.IsNullOrEmpty(Sku) ? ProductId : $"{ProductId}_{Sku}";
    }
}
