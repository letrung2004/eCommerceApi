using AutoMapper;
using MongoDB.Driver;
using ProductService.Presentation.Data;
using ProductService.Presentation.Entities;
using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Categories.GetCategories
{
    public class GetCategoriesHandler
    {
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;

        public GetCategoriesHandler(MongoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PageResponse<CategoryResponse>> HandleAsync(GetCategoriesQuery query)
        {
            var pageNumber = query.PageNumber ?? 1;
            var pageSize = query.PageSize ?? 10;
            var filter = Builders<Category>.Filter.Eq(p => p.IsDeleted, false); // tạo một bộ lọc rỗng cho colection Category

            if (!string.IsNullOrEmpty(query.Search))
                filter &= Builders<Category>.Filter.Text(query.Search);

            //tổng lượng danh mục
            var total = (int)await _context.Categories.CountDocumentsAsync(filter);

            //lấy danh sách và phân trang bằng các cú pháp LINQ
            var categories = await _context.Categories
                .Find(filter)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Limit(query.PageSize)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<CategoryResponse>>(categories);

            return new PageResponse<CategoryResponse>(result, total, pageNumber, pageSize);
        }
    }
}
