using MassTransit;
using OrderSaga.Worker.Orchestrator.Interfaces;
using SharedLibrarySolution.Events;

// đây là class để lắng nghe các event được client gửi lên để thực hiện xử lý gọi đến orchestrator
namespace OrderSaga.Worker.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedIntegrationEvent>
    {
        private readonly ISagaOrchestrator _orchestrator;

        public OrderCreatedConsumer(ISagaOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }
        public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
        {
            await _orchestrator.StartOrderProcessingSaga(context.Message, context.CancellationToken);
        }
    }
    
}
