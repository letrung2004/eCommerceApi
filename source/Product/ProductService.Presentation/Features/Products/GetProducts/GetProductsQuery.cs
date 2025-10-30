namespace ProductService.Presentation.Features.Products.GetProducts
{
    public class GetProductsQuery
    {
        public string? Search { get; set; }
        public string? CategoryId { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}
