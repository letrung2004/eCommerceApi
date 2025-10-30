using MongoDB.Driver;
using ProductService.Presentation.Entities;

namespace ProductService.Presentation.Data.Configurations
{
    public static class ProductConfiguration
    {
       
        public static async Task ConfigureAsync(IMongoDatabase database)
        {
            var collection = database.GetCollection<Product>("Products"); // lấy collection Product trong DB và ánh xạ thành đối tượng Product trong C#

            // tạo chỉ mục cho các trường name và description để tìm kiếm theo từ khóa (full-text search) nhanh hơn.
            var textIndex = new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Text(p => p.Name).Text(p => p.Description),
                new CreateIndexOptions { Name = "TextSearchIndex" });

            // Tạo chỉ mục kết hợp (compound index) trên CategoryId và Price.
            // Giúp tối ưu truy vấn khi bạn lọc sản phẩm theo danh mục và sắp xếp hoặc lọc theo giá.
            var compoundIndex = new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys
                    .Ascending(p => p.CategoryId)
                    .Ascending(p => p.Price),
                new CreateIndexOptions { Name = "CategoryPriceIndex" });

            await collection.Indexes.CreateManyAsync(new[] { textIndex, compoundIndex });
        }
    }
}
