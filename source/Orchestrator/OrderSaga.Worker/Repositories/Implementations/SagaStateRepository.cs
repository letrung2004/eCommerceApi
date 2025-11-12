using Microsoft.EntityFrameworkCore;
using OrderSaga.Worker.Data;
using OrderSaga.Worker.Entities;
using OrderSaga.Worker.Repositories.Interfaces;

namespace OrderSaga.Worker.Repositories.Implementations
{
    public class SagaStateRepository : ISagaStateRepository
    {
        private readonly SagaDbContext _dbContext;

        public SagaStateRepository(SagaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // lấy danh sách các saga có trạng thái pending
        public async Task<IEnumerable<OrderSagaState>> GetPendingSagasAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.OrderSagaStates
                .Where(s => s.Status == Enums.SagaStatus.Started || s.Status == Enums.SagaStatus.Failed)
                .ToListAsync(cancellationToken);
        }

        // lấy saga theo order id
        public async Task<OrderSagaState?> GetSagaStateAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.OrderSagaStates.FirstOrDefaultAsync(s => s.OrderId == orderId, cancellationToken);
        }

        // lưu trạng thái saga mới
        public async Task SaveSagaStateAsync(OrderSagaState state, CancellationToken cancellationToken = default)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            await _dbContext.OrderSagaStates.AddAsync(state, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // cập nhật trạng thái saga
        public async Task UpdateSagaStateAsync(OrderSagaState state, CancellationToken cancellationToken = default)
        {
            if(state == null) throw new ArgumentNullException(nameof(state));
            _dbContext.OrderSagaStates.Update(state);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
