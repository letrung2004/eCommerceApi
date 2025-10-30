namespace BasketService.Presentation.Features.Baskets.AddItemToBasket
{
    public class AddItemRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Sku { get; set; } // ✅ null nếu không có variant
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; } = 0;
    }
}
// con validate dùng chung chưa cấu hình