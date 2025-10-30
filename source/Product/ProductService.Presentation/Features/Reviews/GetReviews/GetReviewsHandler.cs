using AutoMapper;
using MongoDB.Driver;
using ProductService.Presentation.Data;
using ProductService.Presentation.Entities;
using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Reviews.GetReviews
{
    public class GetReviewsHandler
    {
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;

        public GetReviewsHandler(MongoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PageResponse<ReviewResponse>> HandleAsync(GetReviewsQuery query)
        {
            var pageNumber = query.PageNumber ?? 1;
            var pageSize = query.PageSize ?? 10;

            var filter = Builders<Review>.Filter.Eq(r => r.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(query.Comment))
                filter &= Builders<Review>.Filter.Text(query.Comment);

            if (query.Rating.HasValue)
                filter &= Builders<Review>.Filter.Eq(r => r.Rating, query.Rating.Value);

            var total = (int)await _context.Reviews.CountDocumentsAsync(filter);

            var reviews = await _context.Reviews
                .Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<ReviewResponse>>(reviews);

            return new PageResponse<ReviewResponse>(result, total, pageNumber, pageSize);
        }
    }
}
