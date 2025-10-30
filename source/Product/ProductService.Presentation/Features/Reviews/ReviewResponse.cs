using ProductService.Presentation.Entities;
using SharedLibrarySolution.Interfaces;

namespace ProductService.Presentation.Features.Reviews
{
    public class ReviewResponse : IMapFrom<Review>
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
