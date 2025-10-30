using AutoMapper;
using BasketService.Presentation.Entities;
using Newtonsoft.Json;
using SharedLibrarySolution.Exceptions;
using StackExchange.Redis;
using System.Security.Claims;

namespace BasketService.Presentation.Features.Baskets.GetBasket
{
    public class GetBasketHandler
    {
        private readonly IDatabase _redis;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;

        public GetBasketHandler(IConnectionMultiplexer redis, IHttpContextAccessor httpContext, IMapper mapper)
        {
            _redis = redis.GetDatabase();
            _httpContext = httpContext;
            _mapper = mapper;
        }

        public async Task<BasketResponse> HandleAsync()
        {
            try
            {
                var userId = _httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    throw new AppException("Unauthorize");

                var key = $"basket:{userId}";

                // Đọc dạng string (vì AddItemHandler lưu bằng StringSetAsync)
                var basketJson = await _redis.StringGetAsync(key);

                if (string.IsNullOrEmpty(basketJson))
                {
                    return new BasketResponse
                    {
                        UserId = userId,
                        Items = new List<BasketItemResponse>()
                    };
                }

                // Giải mã JSON thành entity
                var basket = JsonConvert.DeserializeObject<BasketService.Presentation.Entities.Basket>(basketJson!);

                if (basket == null)
                {
                    return new BasketResponse
                    {
                        UserId = userId,
                        Items = new List<BasketItemResponse>()
                    };
                }

                // Map sang DTO
                var basketResponse = _mapper.Map<BasketResponse>(basket);
                basketResponse.TotalPrice = basket.TotalPrice;
                return basketResponse;
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
            }
        }
    }
}
