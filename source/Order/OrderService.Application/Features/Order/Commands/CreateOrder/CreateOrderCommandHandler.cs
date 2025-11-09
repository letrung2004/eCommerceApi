using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Order.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
    {
        public Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

// flow create order

//[User]
//   ↓ (HTTP)
//[OrderService.Presentation]
//   ↓
//(gRPC sync)-- > [InventoryService.CheckInventory]-- > true / false
//[CreatedOrderCommandHandler]--publish-- > OrderCreatedEvent(RabbitMQ / MassTransit)-- > SagaOrchestrator
//SagaOrchestrator:
//1.Call InventoryService.ReserveInventory(async)-- > commit DB-- > publish InventoryReservedEvent
//    2. Call PaymentService.PreAuthorize (async) --> commit DB --> publish PaymentAuthorizedEvent
//    3. Confirm Order --> commit DB --> publish OrderCompletedEvent
//    4. Nếu fail step nào -> rollback tương ứng --> publish InventoryReleased / PaymentFailed / OrderCancelled

