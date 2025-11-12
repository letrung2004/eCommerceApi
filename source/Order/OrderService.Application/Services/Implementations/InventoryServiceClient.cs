using InventoryService.gRPC;
using OrderService.Application.Services.Interfaces;


namespace OrderService.Application.Services.Implementations
{
    public class InventoryServiceClient : IInventoryServiceClient
    {
        private readonly InventoryService.gRPC.Inventory.InventoryClient _grpcClient;

        public InventoryServiceClient(InventoryService.gRPC.Inventory.InventoryClient grpcClient)
        {
            _grpcClient = grpcClient;
        }
        public async Task<bool> CheckStockAsync(string productId, int quantity)
        {
            var res = await _grpcClient.CheckInventoryAsync(new CheckInventoryRequest
            {
                ProductId = productId,
                Quantity = quantity
            });
            return res.IsAvailable;
        }

        //public async Task ConfirmStockAsync(string productId)
        //{
        //    var res = await _grpcClient.ConfirmInventoryAsync(new ConfirmInventoryRequest { ProductId = productId });
        //}

        //public async Task ReleaseStockAsync(string productId, int quantity)
        //{
        //    await _grpcClient.ReleaseInventoryAsync(new ReleaseInventoryRequest
        //    {
        //        ProductId = productId,
        //        Quantity = quantity
        //    });
        //}

        //public async Task<bool> ReserveStockAsync(string productId, int quantity)
        //{
        //    var response = await _grpcClient.ReserveInventoryAsync(new ReserveInventoryRequest
        //    {
        //        ProductId = productId,
        //        Quantity = quantity
        //    });
        //    return response.Success;
        //}
    }
}


// 09/11/2025
// viết client requet đến server, viết logic order