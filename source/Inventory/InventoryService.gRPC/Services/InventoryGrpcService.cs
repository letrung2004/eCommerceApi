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
        public override async Task<CheckInventoryReply> CheckInventory(CheckInventoryRequest request, ServerCallContext context)
        {
            var isAvailable = await _inventoryService.CheckAvailability(request.ProductId, request.Quantity);
            return new CheckInventoryReply { IsAvailable = isAvailable };
        }
        public override async Task<ReserveInventoryReply> ReserveInventory(ReserveInventoryRequest request, ServerCallContext context)
        {
            var success = await _inventoryService.ReserveItem(request.ProductId, request.Quantity);
            return new ReserveInventoryReply { Success = success };
        }
        public override async Task<EmptyReply> ReleaseInventory(ReleaseInventoryRequest request, ServerCallContext context)
        {
            await _inventoryService.ReleaseItem(request.ProductId, request.Quantity);
            return new EmptyReply();
        }
        public override async Task<EmptyReply> ConfirmInventory(ConfirmInventoryRequest request, ServerCallContext context)
        {
            await _inventoryService.ConfirmItem(request.ProductId);
            return new EmptyReply();
        }
    }
}
