using MassTransit;
using OrderSaga.Worker.Orchestrator.Interfaces;
using SharedLibrarySolution.Events;

// Consumer để lắng nghe OrderCreatedIntegrationEvent và trigger Saga workflow
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
            try
            {
                var @event = context.Message;

                //  Log chi tiết khi nhận được event
                Console.WriteLine("========================================");
                Console.WriteLine("SAGA RECEIVED EVENT");
                Console.WriteLine($"   OrderId: {@event.OrderId}");
                Console.WriteLine($"   UserId: {@event.UserId}");
                Console.WriteLine($"   TotalPrice: ${@event.TotalPrice}");
                Console.WriteLine("========================================");

                _logger.LogInformation(
                    "Received OrderCreatedIntegrationEvent: OrderId={OrderId}, UserId={UserId}, Total={Total}",
                    @event.OrderId, @event.UserId, @event.TotalPrice
                );

                // Gọi Orchestrator để bắt đầu Saga
                await _orchestrator.StartOrderProcessingSaga(@event, context.CancellationToken);

                Console.WriteLine($"Saga completed successfully for OrderId={@event.OrderId}");
                _logger.LogInformation("Saga completed for OrderId={OrderId}", @event.OrderId);
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                Console.WriteLine("========================================");
                Console.WriteLine($" SAGA FAILED");
                Console.WriteLine($"   OrderId: {context.Message.OrderId}");
                Console.WriteLine($"   Error: {ex.Message}");
                Console.WriteLine($"   StackTrace: {ex.StackTrace}");
                Console.WriteLine("========================================");

                _logger.LogError(ex, " Saga failed for OrderId={OrderId}", context.Message.OrderId);

                //  Re-throw để MassTransit retry hoặc move to error queue
                throw;
            }
        }
    }
}