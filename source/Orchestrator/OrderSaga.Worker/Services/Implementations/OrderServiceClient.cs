using OrderSaga.Worker.DTOs;
using OrderSaga.Worker.Services.Interfaces;
using SharedLibrarySolution.Responses;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderSaga.Worker.Services.Implementations
{
    public class OrderServiceClient : IOrderServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderServiceClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public OrderServiceClient(HttpClient httpClient, ILogger<OrderServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<OrderItemDto>> GetOrderItemByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/orders/{orderId}/items", cancellationToken);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<OrderItemDto>>>(_jsonOptions, cancellationToken);
                return result?.Result ?? new List<OrderItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting order items for {OrderId}", orderId);
                return new List<OrderItemDto>();
            }
        }

        public async Task<bool> MarkOrderAsProcessingAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/orders/{orderId}/mark-processing", null, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking order {OrderId} as processing", orderId);
                return false;
            }
        }

        public async Task<bool> MarkOrderAsPaidAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/orders/{orderId}/mark-paid", null, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking order {OrderId} as paid", orderId);
                return false;
            }
        }

        public async Task<bool> MarkOrderAsCancelledAsync(Guid orderId, string reason, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = $"api/orders/{orderId}/cancel";
                var content = JsonContent.Create(new { Reason = reason });

                var response = await _httpClient.PutAsync(requestUri, content, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
                return false;
            }
        }
    }
}
