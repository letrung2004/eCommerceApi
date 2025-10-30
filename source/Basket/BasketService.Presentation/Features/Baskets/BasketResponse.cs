using BasketService.Presentation.Entities;
using SharedLibrarySolution.Interfaces;

namespace BasketService.Presentation.Features.Baskets
{
    public class BasketResponse : IMapFrom<BasketService.Presentation.Entities.Basket>
    {
        public string UserId { get; set; } = string.Empty;
        public List<BasketItemResponse> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }

    public class BasketItemResponse : IMapFrom<BasketItem>
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Sku { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal Subtotal => Quantity * (DiscountPrice > 0 ? DiscountPrice : Price);
    }
}
