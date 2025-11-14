using PaymentService.gRPC.Application.Interfaces.IRepositories;
using PaymentService.gRPC.Domain.Entities;
using PaymentService.gRPC.Domain.Enums;

namespace PaymentService.gRPC.Application.Services
{
    public class PaymentAppService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentAppService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Payment> CreatePaymentAsync(Guid orderId, decimal amount)
        {
            var payment = new Payment(orderId, amount);
            await _paymentRepository.AddAsync(payment);
            return payment;
        }

        public async Task<Payment> ProcessPaymentAsync(Guid paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException("Payment không tồn tại.");

            // thanh toán thật tại đây -  gọi đến các cổng xử lý thanh toán

            payment.MarkProcessed();
            await _paymentRepository.UpdateAsync(payment);
            return payment;
        }

        //public async Task<Payment> RefundPaymentAsync(Guid paymentId)
        //{
        //    var payment = await _paymentRepository.GetByIdAsync(paymentId);
        //    if (payment == null)
        //        throw new KeyNotFoundException("Payment không tồn tại.");

        //    payment.MarkRefunded();
        //    await _paymentRepository.UpdateAsync(payment);
        //    return payment;
        //}

        public async Task<PaymentStatus?> GetPaymentStatusAsync(Guid paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            return payment?.Status;
        }

        public async Task<Payment> MarkFailedAsync(Guid paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException("Payment không tồn tại.");

            payment.MarkFailed();
            await _paymentRepository.UpdateAsync(payment);
            return payment;
        }

        public async Task<bool> UpdatePaymentStatusAsync(Guid paymentId, string newStatus)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                return false; // hoặc throw KeyNotFoundException tùy bạn muốn handle

            // Convert string sang enum PaymentStatus
            if (!Enum.TryParse<PaymentStatus>(newStatus, out var statusEnum))
                throw new ArgumentException("Invalid payment status");

            payment.Status = statusEnum;
            await _paymentRepository.UpdateAsync(payment);

            return true;
        }

    }
}
