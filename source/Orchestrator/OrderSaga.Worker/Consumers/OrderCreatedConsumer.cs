using MassTransit;
using OrderSaga.Worker.Orchestrator.Interfaces;
using SharedLibrarySolution.Events;

// đây là class để lắng nghe các event được client gửi lên để thực hiện xử lý gọi đến orchestrator
namespace OrderSaga.Worker.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedIntegrationEvent>
    {
        private readonly ISagaOrchestrator _orchestrator;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ISagaOrchestrator orchestrator, ILogger<OrderCreatedConsumer> logger)
        {
            _orchestrator = orchestrator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
        {
            var @event = context.Message;

            Console.WriteLine($"🎯 SAGA RECEIVED => OrderId={@event.OrderId}, UserId={@event.UserId}, Total=${@event.TotalPrice}");

            _logger.LogInformation("📦 Received OrderCreatedIntegrationEvent: OrderId={OrderId}", @event.OrderId);

            try
            {
                await _orchestrator.StartOrderProcessingSaga(@event, context.CancellationToken);

                _logger.LogInformation("✅ Saga completed for OrderId={OrderId}", @event.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Saga failed for OrderId={OrderId}", @event.OrderId);
                throw;
            }
        }
    }
    
}
