using System.Text.Json.Serialization;

//do có gửi ảnh sản phẩm nên cần gửi fromdata các trường dưới đây xử lý để hỗ trợ cho việc đó tiện hơn
namespace ProductService.Presentation.Features.Products.CreateProduct
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryId { get; set; } = string.Empty;

        //JSON string cho Variants (bind được với FormData)
       public string VariantsJson { get; set; } = "[]";

        //  cho phép up nhiều ảnh
        public IFormFileCollection? Images { get; set; }
    }

    //DTO nội bộ để deserialize VariantsJson
    public class ProductVariantRequestDto
    {
        [JsonPropertyName("sku")]
        public string Sku { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

    }
}