using PaymentService.gRPC.Domain.Entities;

namespace PaymentService.gRPC.Application.Interfaces.IRepositories
{
    public interface IRefundRepository
    {
        Task AddAsync(Refund refund);
        Task<Refund?> GetByIdAsync(Guid id);
        Task<IEnumerable<Refund>> GetByOrderIdAsync(Guid orderId);
        Task UpdateAsync(Refund refund);
        Task SaveChangesAsync();
    }
}
