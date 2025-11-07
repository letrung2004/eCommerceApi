using Grpc.Core;
using PaymentService.gRPC.Application.Services;
using PaymentService.gRPC.Protos;

namespace PaymentService.gRPC.Services
{
    public class RefundGrpcService : RefundService.RefundServiceBase
    {
        private readonly RefundAppService _refundAppService;

        public RefundGrpcService(RefundAppService refundAppService)
        {
            _refundAppService = refundAppService;
        }

        public override async Task<RefundResponse> RequestRefund(RefundRequest request, ServerCallContext context)
        {
            var refund = await _refundAppService.RequestRefundAsync(
                Guid.Parse(request.OrderId),
                (decimal)request.Amount
            );

            return new RefundResponse
            {
                RefundId = refund.Id.ToString(),
                OrderId = refund.OrderId.ToString(),
                Amount = (double)refund.Amount,
                Status = refund.Status.ToString(),
                RequestedAt = refund.RequestedAt.ToString("O")
            };
        }

        public override async Task<RefundResponse> ApproveRefund(RefundActionRequest request, ServerCallContext context)
        {
            var refund = await _refundAppService.ApproveRefundAsync(Guid.Parse(request.RefundId));
            return MapRefund(refund);
        }

        public override async Task<RefundResponse> RejectRefund(RefundActionRequest request, ServerCallContext context)
        {
            var refund = await _refundAppService.RejectRefundAsync(Guid.Parse(request.RefundId));
            return MapRefund(refund);
        }

        public override async Task<RefundResponse> GetRefundById(RefundGetRequest request, ServerCallContext context)
        {
            var refund = await _refundAppService.GetRefundByIdAsync(Guid.Parse(request.RefundId));
            if (refund == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Refund not found"));

            return MapRefund(refund);
        }

        private static RefundResponse MapRefund(Domain.Entities.Refund refund) => new RefundResponse
        {
            RefundId = refund.Id.ToString(),
            OrderId = refund.OrderId.ToString(),
            Amount = (double)refund.Amount,
            Status = refund.Status.ToString(),
            RequestedAt = refund.RequestedAt.ToString("O")
        };
    }
}
