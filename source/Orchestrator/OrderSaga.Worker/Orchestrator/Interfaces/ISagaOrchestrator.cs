using SharedLibrarySolution.Events;

namespace OrderSaga.Worker.Orchestrator.Interfaces
{
    public interface ISagaOrchestrator
    {
        /// <summary>
        /// Bắt đầu xử lý Saga cho đơn hàng mới.
        /// </summary>
        Task StartOrderProcessingSaga(OrderCreatedIntegrationEvent orderEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tiếp tục xử lý các Saga đang ở trạng thái lỗi hoặc chưa hoàn thành.
        /// </summary>
        Task ResumeFailSagaAsync(CancellationToken cancellationToken = default);
    }
}
