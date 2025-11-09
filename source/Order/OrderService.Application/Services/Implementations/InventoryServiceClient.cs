using OrderService.Application.Services.Interfaces;
using InventoryService.gRPC;


namespace OrderService.Application.Services.Implementations
{
    internal class InventoryServiceClient : IInventoryServiceClient
    {
        private readonly InventoryService.gRPC.Inventory.InventoryClient _grpcClient;

        public InventoryServiceClient(InventoryService.gRPC.Inventory.InventoryClient grpcClient)
        {
            _grpcClient = grpcClient;
        }
        public Task<bool> CheckStockAsync(string productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task ConfirmStockAsync(string productId)
        {
            throw new NotImplementedException();
        }

        public Task ReleaseStockAsync(string productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReserveStockAsync(string productId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}


// 09/11/2025
// viết client requet đến server, viết logic order