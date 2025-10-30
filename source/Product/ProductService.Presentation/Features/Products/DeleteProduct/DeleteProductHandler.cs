using MongoDB.Driver;
using ProductService.Presentation.Data;
using ProductService.Presentation.Entities;


namespace ProductService.Presentation.Features.Products.DeleteProduct
{
    public class DeleteProductHandler
    {
        private readonly MongoDbContext _context;

        public DeleteProductHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HandleAsync(DeleteProductRequest request)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, request.Id);
            var deletedProduct = await _context.Products.FindOneAndDeleteAsync(filter);

            return deletedProduct != null;
        }
    }
}
