using MassTransit;
using ProductService.Presentation.Entities.Events;
using SharedLibrarySolution.Exceptions;
using StackExchange.Redis;
using System.Text.Json;

namespace BasketService.Presentation.Features.Baskets.Consumers
{
    public class ProductUpdatedConsumer : IConsumer<ProductUpdatedEvent>
    {
        private readonly IDatabase _redis;
        private readonly ILogger<ProductUpdatedConsumer> _logger;

        public ProductUpdatedConsumer(IConnectionMultiplexer redis, ILogger<ProductUpdatedConsumer> logger)
        {
            _redis = redis.GetDatabase();
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
        {
            var @event = context.Message;

            Console.WriteLine($"Received ProductUpdatedEvent => ProductId={@event.ProductId}, Name=\"{@event.ProductName}\", Price={@event.Price}");


            try
            {
                var productUsersKey = $"product:{@event.ProductId}:users";
                var userIds = await _redis.SetMembersAsync(productUsersKey);

                if (userIds.Length == 0)
                {
                    Console.WriteLine("No baskets contain ProductId={ProductId}", @event.ProductId);
                    return;
                }

                var updatedCount = 0;

                foreach (var userId in userIds)
                {
                    var basketKey = $"basket:{userId}";
                    var basketJson = await _redis.StringGetAsync(basketKey);

                    if (string.IsNullOrEmpty(basketJson))
                    {
                        await _redis.SetRemoveAsync(productUsersKey, userId);
                        continue;
                    }

                    var basket = JsonSerializer.Deserialize<BasketService.Presentation.Entities.Basket>(basketJson!);
                    if (basket == null) continue;

                    // ✅ Update mọi item có ProductId trùng
                    var itemsToUpdate = basket.Items
                        .Where(i => i.ProductId == @event.ProductId)
                        .ToList();

                    if (itemsToUpdate.Any())
                    {
                        foreach (var item in itemsToUpdate)
                        {
                            item.ProductName = @event.ProductName;
                            item.Price = @event.Price;
                        }

                        basket.UpdatedAt = DateTime.UtcNow;

                        var updatedJson = JsonSerializer.Serialize(basket);
                        await _redis.StringSetAsync(basketKey, updatedJson, basket.TTL);

                        updatedCount++;
                    }
                }

                //Console.WriteLine("Updated {Count} baskets for ProductId={ProductId}",
                //    updatedCount, @event.ProductId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error processing ProductUpdatedEvent:" +ex);
                throw new AppException(" Error processing ProductUpdatedEvent:" + ex);
            }
        }
    }
}
