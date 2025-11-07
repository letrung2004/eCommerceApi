using PaymentService.gRPC.Application.Interfaces.IRepositories;
using PaymentService.gRPC.Domain.Entities;
using SharedLibrarySolution.Exceptions;

namespace PaymentService.gRPC.Application.Services
{
    public class RefundAppService
    {
        private readonly IRefundRepository _refundRepository;

        public RefundAppService(IRefundRepository refundRepository)
        {
            _refundRepository = refundRepository;
        }

        public async Task<Refund> RequestRefundAsync(Guid orderId, decimal amount)
        {
            var refund = new Refund(Guid.NewGuid(), orderId, amount);
            await _refundRepository.AddAsync(refund);
            return refund;
        }

        public async Task<Refund> ApproveRefundAsync(Guid refundId)
        {
            var refund = await _refundRepository.GetByIdAsync(refundId)
                ?? throw new AppException($"Không tìm thấy Refund với id {refundId}");

            refund.ApproveRefund();
            await _refundRepository.UpdateAsync(refund);
            return refund;
        }

        public async Task<Refund> RejectRefundAsync(Guid refundId)
        {
            var refund = await _refundRepository.GetByIdAsync(refundId)
                ?? throw new AppException($"Không tìm thấy Refund với id {refundId}");

            refund.RejectRefund();
            await _refundRepository.UpdateAsync(refund);
            return refund;
        }

        public async Task<Refund?> GetRefundByIdAsync(Guid refundId)
        {
            return await _refundRepository.GetByIdAsync(refundId);
        }
    }
}
