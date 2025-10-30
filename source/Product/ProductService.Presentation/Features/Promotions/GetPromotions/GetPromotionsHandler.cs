using AutoMapper;
using MongoDB.Driver;
using ProductService.Presentation.Data;
using ProductService.Presentation.Entities;
using ProductService.Presentation.Features.Categories;
using ProductService.Presentation.Features.Categories.GetCategories;
using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Promotions.GetPromotions
{
    public class GetPromotionsHandler
    {
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;

        public GetPromotionsHandler(MongoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PageResponse<PromotionResponse>> HandleAsync(GetPromotionsQuery query)
        {
            var pageNumber = query.PageNumber ?? 1;
            var pageSize = query.PageSize ?? 10;
            var filter = Builders<Promotion>.Filter.Eq(p => p.IsDeleted, false); // tạo một bộ lọc rỗng cho colection Category

            if (!string.IsNullOrEmpty(query.Search))
                filter &= Builders<Promotion>.Filter.Text(query.Search);

            if (!string.IsNullOrEmpty(query.TargetId))
            {
                filter &= Builders<Promotion>.Filter.Eq(p => p.TargetId, query.TargetId);
            }

            //tổng lượng danh mục
            var total = (int)await _context.Promotions.CountDocumentsAsync(filter);

            //lấy danh sách và phân trang bằng các cú pháp LINQ
            var promotions = await _context.Promotions
                .Find(filter)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Limit(query.PageSize)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<PromotionResponse>>(promotions);

            return new PageResponse<PromotionResponse>(result, total, pageNumber, pageSize);
        }
    }
}
