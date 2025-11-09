using Grpc.Core;

//Expose API cho OrderService

namespace InventoryService.gRPC.Services
{
    public class InventoryGrpcService : Inventory.InventoryBase
    {
        private readonly Application.Services.InventoryService _inventoryService;

        public InventoryGrpcService(Application.Services.InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // Check stock nhanh khi tạo order
        public override async Task<CheckInventoryReply> CheckInventory(CheckInventoryRequest request, ServerCallContext context)
        {
            var isAvailable = await _inventoryService.CheckAvailability(request.ProductId, request.Quantity);
            return new CheckInventoryReply { IsAvailable = isAvailable };
        }

        // Reserve stock khi order tạo thành công
        public override async Task<ReserveInventoryReply> ReserveInventory(ReserveInventoryRequest request, ServerCallContext context)
        {
            var success = await _inventoryService.ReserveItem(request.ProductId, request.Quantity);
            return new ReserveInventoryReply { Success = success };
        }

        // Release stock khi order bị hủy hoặc thanh toán thất bại
        public override async Task<EmptyReply> ReleaseInventory(ReleaseInventoryRequest request, ServerCallContext context)
        {
            var success = await _inventoryService.ReleaseItem(request.ProductId, request.Quantity);
            if (!success)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
            }
            return new EmptyReply();
        }

        // Confirm stock khi order thành công
        public override async Task<EmptyReply> ConfirmInventory(ConfirmInventoryRequest request, ServerCallContext context)
        {
            await _inventoryService.ConfirmItem(request.ProductId);
            return new EmptyReply();
        }
    }
}
