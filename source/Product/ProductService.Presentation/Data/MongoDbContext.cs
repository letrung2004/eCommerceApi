using MongoDB.Driver;
using ProductService.Presentation.Entities;

namespace ProductService.Presentation.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
            var dbName = configuration["DatabaseSettings:DatabaseName"];

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(dbName);
            EnsureCollectionsExist();
        }

      
        // Đảm bảo tạo db nếu chưa có
        private void EnsureCollectionsExist()
        {
            var collectionNames = _database.ListCollectionNames().ToList();

            if (!collectionNames.Contains("Products"))
                _database.CreateCollection("Products");
            if (!collectionNames.Contains("Categories"))
                _database.CreateCollection("Categories");
            if (!collectionNames.Contains("Promotions"))
                _database.CreateCollection("Promotions");
            if (!collectionNames.Contains("Reviews"))
                _database.CreateCollection("Reviews");
        }

        // Expose các collection như table trong SQL
        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
        public IMongoCollection<Promotion> Promotions => _database.GetCollection<Promotion>("Promotions");
        public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("Reviews");

        public IMongoDatabase Database => _database;
    }
}