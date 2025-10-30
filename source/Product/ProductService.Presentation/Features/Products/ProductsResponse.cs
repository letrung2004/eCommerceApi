using ProductService.Presentation.Entities;
using SharedLibrarySolution.Interfaces;

namespace ProductService.Presentation.Features.Products
{
    public class ProductVariantResponse : IMapFrom<ProductVariant>
    {
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class ProductsResponse : IMapFrom<Product>
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public double RatingAverage { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<ProductVariantResponse> Variants { get; set; } = new(); 
    }
}
