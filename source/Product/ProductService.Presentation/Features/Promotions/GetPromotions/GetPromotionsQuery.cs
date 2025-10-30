namespace ProductService.Presentation.Features.Promotions.GetPromotions
{
    public class GetPromotionsQuery
    {
        public string? Search { get; set; }
        public string? TargetId { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}
