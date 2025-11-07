using Microsoft.EntityFrameworkCore;
using PaymentService.gRPC.Application.Interfaces.IRepositories;
using PaymentService.gRPC.Domain.Entities;
using PaymentService.gRPC.Infrastructure.Data;

namespace PaymentService.gRPC.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentRepository(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync(); // commit luôn
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync(); // commit luôn
        }
    }
}
