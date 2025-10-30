using MongoDB.Driver;
using ProductService.Presentation.Entities;

namespace ProductService.Presentation.Data.Configurations
{
    public static class PromotionConfiguration
    {
        public static async Task ConfigureAsync(MongoDbContext db)
        {
            var collection = db.Promotions;

            // Text index cho field Name để tìm kiếm theo từ khóa
            var textIndex = new CreateIndexModel<Promotion>(
                Builders<Promotion>.IndexKeys.Text(p => p.Name),
                new CreateIndexOptions { Name = "PromotionTextIndex" });

            // Index theo ngày bắt đầu và kết thúc khuyến mãi
            var dateIndex = new CreateIndexModel<Promotion>(
                Builders<Promotion>.IndexKeys
                    .Ascending(p => p.StartDate)
                    .Ascending(p => p.EndDate),
                new CreateIndexOptions { Name = "PromotionDateIndex" });

            await collection.Indexes.CreateManyAsync(new[] { textIndex, dateIndex });
        }
    }
}
