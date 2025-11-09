using Microsoft.EntityFrameworkCore;
using PaymentService.gRPC.Application.Interfaces.IRepositories;
using PaymentService.gRPC.Domain.Entities;
using PaymentService.gRPC.Infrastructure.Data;

namespace PaymentService.gRPC.Infrastructure.Repositories
{
    public class RefundRepository : IRefundRepository
    {
        private readonly PaymentDbContext _context;

        public RefundRepository(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Refund refund)
        {
            await _context.Refunds.AddAsync(refund);
            await _context.SaveChangesAsync(); // commit luôn
        }

        public async Task<Refund?> GetByIdAsync(Guid id)
        {
            return await _context.Refunds.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Refund>> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.Refunds
                .Where(r => r.OrderId == orderId)
                .ToListAsync();
        }

        public async Task UpdateAsync(Refund refund)
        {
            _context.Refunds.Update(refund);
            await Task.CompletedTask;
            await _context.SaveChangesAsync(); // commit luôn
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
