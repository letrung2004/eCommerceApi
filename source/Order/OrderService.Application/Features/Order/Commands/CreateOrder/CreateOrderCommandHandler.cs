using AutoMapper;
using MassTransit;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Application.Services.Interfaces;
using OrderService.Domain.Entities;
using SharedLibrarySolution.Events;
using SharedLibrarySolution.Exceptions;

namespace OrderService.Application.Features.Order.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
    {
        private readonly IInventoryServiceClient _inventoryClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        public CreateOrderCommandHandler(
            IInventoryServiceClient inventoryClient,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            _inventoryClient = inventoryClient;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }


        public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // check input
            if (request.UserId == Guid.Empty)
                throw new AppException("UserId không hợp lệ.");
            if (request.Items == null || !request.Items.Any())
                throw new AppException("Đơn hàng phải có ít nhất một sản phẩm.");

            // check stock gọi inventory service qua grcp
            foreach (var item in request.Items) {
                var available = await _inventoryClient.CheckStockAsync(item.ProductId, item.Quantity);
                if (!available)
                    throw new AppException($"Sản phẩm {item.ProductId} không đủ hàng trong kho.");
            }

            var orderId = Guid.NewGuid();

            // Tạo danh sách OrderItem từ request
            var orderItems = request.Items.Select(i =>
                new OrderItem(orderId, i.ProductId, i.Quantity, i.Price)
            ).ToList();

            // Tạo Order domain entity
            var newOrder = new Domain.Entities.Order(orderId, request.UserId, orderItems);

            // Lưu vào repository
            //lấy repo từ unitofwork
            var orderRepo = _unitOfWork.GetRepository<Domain.Entities.Order>(); // là order repo
            await orderRepo.CreateAsync(newOrder);

            //Commit transaction
            await _unitOfWork.CommitAsync(); //tắc để debug

            // gửi event order create -> payment (if don't success rollback)
            var interationEvent = new OrderCreatedIntegrationEvent(newOrder.Id, newOrder.UserId, newOrder.TotalPrice);
            await _publishEndpoint.Publish(interationEvent, cancellationToken); 
            // mastransit gửi event OrderCreatedIntegrationEvent đi => consummer nào đăng ký nhận event này thì sẽ nhận

            return _mapper.Map<OrderResponse>(newOrder);

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



//[CreateOrderCommandHandler]
//   |
//   |--(gRPC sync)--> InventoryService.CheckInventory()
//   |
//   |--Save Order(status: Pending)
//   |
//   |--Publish--> OrderCreatedEvent
//                     |
//                     +---> [SagaOrchestrator]
//                             |
//                             |--(gRPC async)--> InventoryService.ReserveInventory()
//                             |--(gRPC async)--> PaymentService.PreAuthorize()
//                             |
//                             |--Publish--> InventoryReservedEvent / PaymentAuthorizedEvent ...

