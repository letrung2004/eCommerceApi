using PaymentService.gRPC.Domain.Entities;

namespace PaymentService.gRPC.Application.Interfaces.IRepositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
    }
}