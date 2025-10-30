using AutoMapper;
using MongoDB.Driver;
using ProductService.Presentation.Data;
using SharedLibrarySolution.Exceptions;

namespace ProductService.Presentation.Features.Products.GetProductById
{
    public class GetProductByIdHandler
    {
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(MongoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductsResponse> HandleAsync(string id)
        {
            var productDetail = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (productDetail == null)
                throw new AppException("Product not found");

            var response  = _mapper.Map<ProductsResponse>(productDetail);
            return response;
        }
    }
}
