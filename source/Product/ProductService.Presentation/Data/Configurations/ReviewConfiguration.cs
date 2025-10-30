using MongoDB.Driver;
using ProductService.Presentation.Entities;

namespace ProductService.Presentation.Data.Configurations
{
    public static class ReviewConfiguration
    {
        public static async Task ConfigureAsync(IMongoDatabase database)
        {
            var collection = database.GetCollection<Review>("Reviews");

            // Text index trên Comment để tìm kiếm từ khóa
            var commentTextIndex = new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Text(r => r.Comment),
                new CreateIndexOptions { Name = "ReviewCommentTextIndex" });

            // Index để filter/sort theo Rating
            var ratingIndex = new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Descending(r => r.Rating),
                new CreateIndexOptions { Name = "ReviewRatingIndex" });

            // Index để filter nhanh theo ProductId
            var productIndex = new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Ascending(r => r.ProductId),
                new CreateIndexOptions { Name = "ReviewProductIdIndex" });

            // Tạo tất cả các index cùng lúc
            await collection.Indexes.CreateManyAsync(new[] { commentTextIndex, ratingIndex, productIndex });
        }
    }
}
