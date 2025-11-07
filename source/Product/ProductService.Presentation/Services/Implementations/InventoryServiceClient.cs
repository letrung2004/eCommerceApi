using InventoryService.gRPC;
using ProductService.Presentation.Services.Interfaces;
using static InventoryService.gRPC.Product;

// đây là client wrapper để ProductService gọi gRPC InventoryService
// Tất cả logic liên quan đến gửi request và nhận response đều ở đây
// Giúp ProductService chỉ cần gọi method đơn giản, không cần biết chi tiết gRPC

// luồng hoạt động tổng thể
// ProductService->InventoryServiceClient(wrapper)->gRPC client(_productClient)->InventoryService(gRPC server)
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
