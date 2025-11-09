using Grpc.Core;
using PaymentService.gRPC.Application.Mappings;
using PaymentService.gRPC.Application.Services;
using PaymentService.gRPC.Domain.Entities;


namespace PaymentService.gRPC.Services
{
    public class PaymentGrpcService : PaymentService.PaymentServiceBase
    {
        private readonly PaymentAppService _appService;

        public PaymentGrpcService(PaymentAppService appService)
        {
            _appService = appService;
        }

        // ghi đè lại các phương thức
        public override async Task<PaymentResponse> CreatePayment(CreatePaymentRequest request, ServerCallContext context)
        {
            var payment = await _appService.CreatePaymentAsync(Guid.Parse(request.OrderId), (decimal)request.Amount);
            return PaymentMapper.ToResponse(payment, "Payment created");
        }

        public override async Task<PaymentResponse> UpdatePaymentStatus(UpdatePaymentStatusRequest request, ServerCallContext context)
        {
            var success = await _appService.UpdatePaymentStatusAsync(Guid.Parse(request.PaymentId), request.Status);

            return new PaymentResponse
            {
                Success = success,
                PaymentId = request.PaymentId,
                Message = success ? "Status updated" : "Failed to update status"
            };
        }

        public override async Task<PaymentResponse> ProcessPayment(ProcessPaymentRequest request, ServerCallContext context)
        {
            var p = await _appService.ProcessPaymentAsync(Guid.Parse(request.PaymentId));
            return PaymentMapper.ToResponse(p, "Payment processed");

            
        }

        // Đánh dấu thất bại
        public override async Task<PaymentResponse> MarkPaymentFailed(MarkPaymentFailedRequest request, ServerCallContext context)
        {
            var payment = await _appService.MarkFailedAsync(Guid.Parse(request.PaymentId));
            return PaymentMapper.ToResponse(payment, "Payment marked as failed");
        }

        // Lấy trạng thái thanh toán
        public override async Task<PaymentResponse> GetPaymentStatus(GetPaymentStatusRequest request, ServerCallContext context)
        {
            var status = await _appService.GetPaymentStatusAsync(Guid.Parse(request.PaymentId));
            return new PaymentResponse
            {
                Success = status != null,
                PaymentId = request.PaymentId,
                Message = status?.ToString() ?? "Payment not found"
            };
        }


    }
}
