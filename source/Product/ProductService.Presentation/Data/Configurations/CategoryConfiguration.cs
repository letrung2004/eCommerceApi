using MongoDB.Driver;
using ProductService.Presentation.Entities;

namespace ProductService.Presentation.Data.Configurations
{
    public static class CategoryConfiguration
    {
        public static async Task ConfigureAsync(IMongoDatabase database)
        {
            var collection = database.GetCollection<Category>("Categories");

            // Text index trên Name và Description để tìm kiếm từ khóa
            var textIndex = new CreateIndexModel<Category>(
                Builders<Category>.IndexKeys
                    .Text(c => c.Name),
                new CreateIndexOptions { Name = "CategoryTextSearchIndex" });

            // Index theo ParentId (nếu cần filter theo parent)
            var parentIndex = new CreateIndexModel<Category>(
                Builders<Category>.IndexKeys.Ascending(c => c.ParentId),
                new CreateIndexOptions { Name = "ParentCategoryIndex" });

            await collection.Indexes.CreateManyAsync(new[] { textIndex, parentIndex });
        }
    }
}
