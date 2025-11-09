using Grpc.Core;
using InventoryService.gRPC.Application.Services;

//Expose API cho ProductService - when created new product
namespace InventoryService.gRPC.Services
{
    public class ProductGrpcService : Product.ProductBase
    {
        private readonly ProductAppService _productAppService;

        public ProductGrpcService(ProductAppService productAppService)
        {
            _productAppService = productAppService;
        }

        // Cập nhật số lượng khi product handler gọi đến
        public override async Task<UpdateStockResponse> UpdateStock(UpdateStockRequest request, ServerCallContext context)
        {
            var success = await _productAppService.UpdateQuantityInventory(request.ProductId, request.Quantity);
            return new UpdateStockResponse { Success = success };
        }

        // Tạo stock mới khi product mới được tạo
        public override async Task<CreateProductStockResponse> CreateProductStock(CreateProductStockRequest request, ServerCallContext context)
        {
            var success = await _productAppService.CreateInventory(request.ProductId, request.Quantity);
            return new CreateProductStockResponse { Success = success };
        }
    }
}
