using Grpc.Core;
using InventoryService.gRPC.Application.Services;

//Expose API cho ProductService
namespace InventoryService.gRPC.Services
{
    public class ProductGrpcService : Product.ProductBase
    {
        private readonly ProductAppService _productAppService;

        public ProductGrpcService(ProductAppService productAppService)
        {
            _productAppService = productAppService;
        }
        public async override Task<UpdateStockResponse> UpdateStock(UpdateStockRequest request, ServerCallContext context)
        {
            var success = await _productAppService.UpdateQuantityInventory(request.ProductId, request.Quantity);
            return new UpdateStockResponse { Success = success };
        }
        public override async Task<CreateProductStockResponse> CreateProductStock(CreateProductStockRequest request, ServerCallContext context)
        {
            var success = await _productAppService.CreateInventory(request.ProductId, request.Quantity);
            return new CreateProductStockResponse { Success = success };
        }
    }
}
