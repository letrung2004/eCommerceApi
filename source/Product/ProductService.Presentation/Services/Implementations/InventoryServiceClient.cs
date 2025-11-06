using InventoryService.gRPC;
using ProductService.Presentation.Services.Interfaces;

namespace ProductService.Presentation.Services.Implementations
{
    public class InventoryServiceClient : IInventoryServiceClient
    {
        private readonly InventoryService.gRPC.Product.ProductClient _productClient;
        public InventoryServiceClient(InventoryService.gRPC.Product.ProductClient productClient)
        {
            _productClient = productClient;
        }

        public async Task<bool> CreateInventoryAsync(string productId, int qty)
        {
            var response = await _productClient.CreateProductStockAsync(new CreateProductStockRequest
            {
                ProductId = productId,
                Quantity = qty
            });
            return response.Success;
        }

        public async Task<bool> UpdateInventoryAsync(string productId, int qty)
        {
            var response = await _productClient.UpdateStockAsync(new UpdateStockRequest
            {
                ProductId = productId,
                Quantity = qty
            });
            return response.Success;
        }
    }
}
