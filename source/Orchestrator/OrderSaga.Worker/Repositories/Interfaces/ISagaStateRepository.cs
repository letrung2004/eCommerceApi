using OrderSaga.Worker.Entities;

namespace OrderSaga.Worker.Repositories.Interfaces
{
    public interface ISagaStateRepository
    {
        Task SaveSagaStateAsync(OrderSagaState state, CancellationToken cancellationToken = default);
        Task UpdateSagaStateAsync(OrderSagaState state, CancellationToken cancellationToken = default);

        Task<OrderSagaState?> GetSagaStateAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<OrderSagaState>> GetPendingSagasAsync(CancellationToken cancellationToken = default);
    }
}
