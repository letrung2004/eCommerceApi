using StackExchange.Redis;

namespace BasketService.Presentation.Configurations
{
    public static class RedisConfiguration
    {
        public static IServiceCollection AddRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var redisSettings = configuration.GetSection("RedisSettings");
            var connectionString = redisSettings.GetValue<string>("ConnectionString");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("RedisSettings:ConnectionString",
                    "Redis connection string is missing from configuration.");

            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(connectionString));

            return services;
        }
    }
}
