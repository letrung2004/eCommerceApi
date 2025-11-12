using InventoryService.gRPC;
using OrderSaga.Worker.Services.Interfaces;

namespace OrderSaga.Worker.Services.Implementations
{
    public class InventoryServiceClient : IInventoryServiceClient
    {
        private readonly Inventory.InventoryClient _client;

        public InventoryServiceClient(Inventory.InventoryClient client)
        {
            _client = client;
        }

        public async Task<bool> CheckStockAsync(string productId, int quantity, CancellationToken cancellationToken = default)
        {
            var request = new CheckInventoryRequest
            {
                ProductId = productId.ToString(),
                Quantity = quantity
            };

            var reply = await _client.CheckInventoryAsync(request, cancellationToken: cancellationToken);
            return reply.IsAvailable;
        }

        public async Task<bool> ReserveInventoryAsync(string productId, int quantity, CancellationToken cancellationToken = default)
        {
            var request = new ReserveInventoryRequest
            {
                ProductId = productId.ToString(),
                Quantity = quantity
            };

            var reply = await _client.ReserveInventoryAsync(request, cancellationToken: cancellationToken);
            return reply.Success;
        }

        public async Task ReleaseInventoryAsync(string productId, int quantity, CancellationToken cancellationToken = default)
        {
            var request = new ReleaseInventoryRequest
            {
                ProductId = productId.ToString(),
                Quantity = quantity
            };

            await _client.ReleaseInventoryAsync(request, cancellationToken: cancellationToken);
        }

        public async Task ConfirmInventoryAsync(string productId, CancellationToken cancellationToken = default)
        {
            var request = new ConfirmInventoryRequest
            {
                ProductId = productId.ToString()
            };

            await _client.ConfirmInventoryAsync(request, cancellationToken: cancellationToken);
        }
    }
}
