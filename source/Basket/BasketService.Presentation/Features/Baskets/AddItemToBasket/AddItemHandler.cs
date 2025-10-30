// BasketService.Presentation/Features/Baskets/AddItemToBasket/AddItemHandler.cs
using AutoMapper;
using BasketService.Presentation.Entities;
using SharedLibrarySolution.Exceptions;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

namespace BasketService.Presentation.Features.Baskets.AddItemToBasket
{
    public class AddItemHandler
    {
        private readonly IDatabase _redis;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;

        public AddItemHandler(IConnectionMultiplexer redis, IHttpContextAccessor httpContext, IMapper mapper)
        {
            _redis = redis.GetDatabase();
            _httpContext = httpContext;
            _mapper = mapper;
        }

        public async Task<BasketResponse> HandleAsync(AddItemRequest request)
        {
            var userId = _httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new AppException("Unauthorize");

            var basketKey = $"basket:{userId}";

            // Lấy basket dạng JSON
            var basketJson = await _redis.StringGetAsync(basketKey);
            var basket = string.IsNullOrEmpty(basketJson)
                ? new BasketService.Presentation.Entities.Basket { UserId = Guid.Parse(userId) }
                : JsonSerializer.Deserialize<BasketService.Presentation.Entities.Basket>(basketJson!);

            // Tìm item theo UniqueKey
            var uniqueKey = string.IsNullOrEmpty(request.Sku)
                ? request.ProductId
                : $"{request.ProductId}_{request.Sku}";

            var existingItem = basket!.Items.FirstOrDefault(i => i.UniqueKey == uniqueKey);

            if (existingItem != null)
            {
                // Đã có → Tăng số lượng
                existingItem.Quantity += request.Quantity;
                existingItem.Price = request.Price;
                existingItem.DiscountPrice = request.DiscountPrice;
            }
            else
            {
                // Chưa có → Thêm mới
                basket.Items.Add(new BasketItem
                {
                    ProductId = request.ProductId,
                    ProductName = request.ProductName,
                    Sku = request.Sku,
                    Quantity = request.Quantity,
                    Price = request.Price,
                    DiscountPrice = request.DiscountPrice
                });
            }

            basket.UpdatedAt = DateTime.UtcNow;

            // Lưu vào Redis
            var updatedJson = JsonSerializer.Serialize(basket);
            await _redis.StringSetAsync(basketKey, updatedJson, basket.TTL);

            // Reverse index
            var productUsersKey = $"product:{request.ProductId}:users";
            await _redis.SetAddAsync(productUsersKey, userId);

            var response = _mapper.Map<BasketResponse>(basket);
            response.TotalPrice = basket.TotalPrice;
            return response;
        }
    }
}