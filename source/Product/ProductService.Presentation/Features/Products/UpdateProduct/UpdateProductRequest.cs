namespace ProductService.Presentation.Features.Products.UpdateProduct
{
    public class UpdateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryId { get; set; } = string.Empty;

        // JSON string cho variants 
        public string VariantsJson { get; set; } = "[]";

        // Ảnh mới 
        public IFormFileCollection? Images { get; set; }

        // Cho phép gửi dánh sách các ảnh nếu có xóa bớt hoặc giữ nguyên
        public string? OldImageUrls { get; set; }
    }
}
