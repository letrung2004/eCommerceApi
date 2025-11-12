using OrderSaga.Worker.DTOs;
using OrderSaga.Worker.Services.Interfaces;
using PaymentService.gRPC;

namespace OrderSaga.Worker.Services.Implementations
{
    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly PaymentService.gRPC.PaymentService.PaymentServiceClient _paymentClient;
        private readonly PaymentService.gRPC.RefundService.RefundServiceClient _refundClient;

        public PaymentServiceClient(
            PaymentService.gRPC.PaymentService.PaymentServiceClient paymentClient,
            PaymentService.gRPC.RefundService.RefundServiceClient refundClient)
        {
            _paymentClient = paymentClient;
            _refundClient = refundClient;
        }

        public async Task<RefundDto> ApproveRefundAsync(string refundId, CancellationToken cancellationToken = default)
        {
            var request = new RefundActionRequest { RefundId = refundId };
            var res = await _refundClient.ApproveRefundAsync(request, cancellationToken: cancellationToken);
            return MapToDto(res);
        }

        public async Task<(bool IsSuccessful, string PaymentId, string Message)> CreatePaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default)
        {
            var request = new CreatePaymentRequest
            {
                OrderId = orderId.ToString(),
                Amount = (double)amount
            };
            var response = await _paymentClient.CreatePaymentAsync(request, cancellationToken: cancellationToken);
            return (response.Success, response.PaymentId, response.Message);
        }

        public async Task<(bool IsSuccessful, string Message)> GetPaymentStatusAsync(string paymentId, CancellationToken cancellationToken = default)
        {
            var request = new GetPaymentStatusRequest { PaymentId = paymentId };
            var res = await _paymentClient.GetPaymentStatusAsync(request, cancellationToken: cancellationToken);
            return (res.Success, res.Message);
        }

        public async Task<RefundDto> GetRefundByIdAsync(string refundId, CancellationToken cancellationToken = default)
        {
            var request = new RefundGetRequest { RefundId = refundId };
            var res = await _refundClient.GetRefundByIdAsync(request, cancellationToken: cancellationToken);
            return MapToDto(res);
        }

        public async Task<bool> MarkPaymentFailedAsync(string paymentId, CancellationToken cancellationToken = default)
        {
            var request = new MarkPaymentFailedRequest { PaymentId = paymentId };
            var response = await _paymentClient.MarkPaymentFailedAsync(request, cancellationToken: cancellationToken);
            return response.Success;
        }

        public async Task<bool> ProcessPaymentAsync(string paymentId, CancellationToken cancellationToken = default)
        {
            var request = new ProcessPaymentRequest { PaymentId = paymentId };
            var response = await _paymentClient.ProcessPaymentAsync(request, cancellationToken: cancellationToken);
            return response.Success;
        }

        public async Task<RefundDto> RejectRefundAsync(string refundId, CancellationToken cancellationToken = default)
        {
            var request = new RefundActionRequest { RefundId = refundId };
            var response = await _refundClient.RejectRefundAsync(request, cancellationToken: cancellationToken);
            return MapToDto(response);
        }

        public async Task<RefundDto> RequestRefundAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default)
        {
            var request = new RefundRequest
            {
                OrderId = orderId.ToString(),
                Amount = (double)amount
            };

            var response = await _refundClient.RequestRefundAsync(request, cancellationToken: cancellationToken);
            return MapToDto(response);
        }

        public async Task<bool> UpdatePaymentStatusAsync(string paymentId, string status, CancellationToken cancellationToken = default)
        {
            var request = new UpdatePaymentStatusRequest
            {
                PaymentId = paymentId,
                Status = status
            };

            var response = await _paymentClient.UpdatePaymentStatusAsync(request, cancellationToken: cancellationToken);
            return response.Success;
        }

        private RefundDto MapToDto(RefundResponse response)
        {
            return new RefundDto
            {
                Id = response.RefundId,
                OrderId = response.OrderId,
                Amount = (decimal)response.Amount,
                Status = response.Status,
                RequestedAt = DateTime.TryParse(response.RequestedAt, out var dt) ? dt : default
            };
        }
    }
}
