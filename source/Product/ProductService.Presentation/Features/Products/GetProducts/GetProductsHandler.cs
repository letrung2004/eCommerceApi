using AutoMapper;
using MongoDB.Driver;
using ProductService.Presentation.Data;
using ProductService.Presentation.Entities;
using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Products.GetProducts
{
    public class GetProductsHandler
    {
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;

        public GetProductsHandler(MongoDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PageResponse<ProductsResponse>> HandleAsync(GetProductsQuery query)
        {
            var pageNumber = query.PageNumber ?? 1;
            var pageSize = query.PageSize ?? 10;
            var filter = Builders<Product>.Filter.Eq(p => p.IsDeleted, false); // tạo một bộ lọc rỗng cho colection Product

            if (!string.IsNullOrEmpty(query.Search))
                filter &= Builders<Product>.Filter.Text(query.Search);

            if (!string.IsNullOrEmpty(query.CategoryId))
            {
                filter &= Builders<Entities.Product>.Filter.Eq(p => p.CategoryId, query.CategoryId);
            }

            //tổng sản phẩm
            var total = (int)await _context.Products.CountDocumentsAsync(filter);

            //lấy danh sách và phân trang bằng các cú pháp LINQ
            var products = await _context.Products
                .Find(filter)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Limit(query.PageSize)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<ProductsResponse>>(products);

            return new PageResponse<ProductsResponse>(result, total, pageNumber, pageSize);
        }
    }
}
