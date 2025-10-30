using ProductService.Presentation.Data.Configurations;

namespace ProductService.Presentation.Data
{
    public static class MongoDbInitializer
    {
        //đăng ký để gọi các cấu hình
        public static async Task InitializeAsync(MongoDbContext context)
        {
            var db = context.Database;

            await ProductConfiguration.ConfigureAsync(db);
            await CategoryConfiguration.ConfigureAsync(db);
            await PromotionConfiguration.ConfigureAsync(context);
        }
    }
}
