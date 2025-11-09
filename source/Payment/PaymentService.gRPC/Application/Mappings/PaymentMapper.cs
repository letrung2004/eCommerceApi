using PaymentService.gRPC.Domain.Entities;

namespace PaymentService.gRPC.Application.Mappings
{
    public class PaymentMapper
    {
        public static PaymentResponse ToResponse(Payment payment, string message = "OK", bool success = true)
        {
            return new PaymentResponse
            {
                Success = success,
                PaymentId = payment.Id.ToString(),
                Message = message
            };
        }
    }
}
