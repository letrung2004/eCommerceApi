using ProductService.Presentation.Entities;
using SharedLibrarySolution.Interfaces;

namespace ProductService.Presentation.Features.Promotions
{
    public class PromotionResponse : IMapFrom<Promotion>
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AppliesToType { get; set; } = string.Empty; // "Product" hoặc "Category"
        public string TargetId { get; set; } = string.Empty; // ID của sản phẩm hoặc danh mục
        public decimal Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
