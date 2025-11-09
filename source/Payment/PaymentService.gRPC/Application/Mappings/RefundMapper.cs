using PaymentService.gRPC.Domain.Entities;

namespace PaymentService.gRPC.Application.Mappings
{
    public class RefundMapper
    {
        public static RefundResponse ToResponse(Refund refund)
        {
            return new RefundResponse
            {
                RefundId = refund.Id.ToString(),
                OrderId = refund.OrderId.ToString(),
                Amount = (double)refund.Amount,
                Status = refund.Status.ToString(),
                RequestedAt = refund.RequestedAt.ToString("O")
            };
        }
    }
}
