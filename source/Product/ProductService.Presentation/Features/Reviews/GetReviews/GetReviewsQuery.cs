namespace ProductService.Presentation.Features.Reviews.GetReviews
{
    public class GetReviewsQuery
    {
        public int? Rating { get; set; }
        public string? Comment { get; set; } = string.Empty;
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}
