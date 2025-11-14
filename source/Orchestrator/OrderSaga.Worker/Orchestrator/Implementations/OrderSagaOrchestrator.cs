using InventoryService.gRPC;
using OrderSaga.Worker.Entities;
using OrderSaga.Worker.Enums;
using OrderSaga.Worker.Orchestrator.Interfaces;
using OrderSaga.Worker.Repositories.Interfaces;
using OrderSaga.Worker.Services.Interfaces;
using SharedLibrarySolution.Events;

namespace OrderSaga.Worker.Orchestrator.Implementations
{
    public class OrderSagaOrchestrator : ISagaOrchestrator
    {
        private readonly ISagaStateRepository stateRepository;
        private readonly ILogger<OrderSagaOrchestrator> _logger;
        private readonly IOrderServiceClient _orderServiceClient;
        private readonly IInventoryServiceClient _inventoryServiceClient;
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IServiceProvider _serviceProvider;


        public OrderSagaOrchestrator(
             ILogger<OrderSagaOrchestrator> logger,
            IOrderServiceClient orderServiceClient,
            IInventoryServiceClient inventoryServiceClient,
            IPaymentServiceClient paymentServiceClient,
            IServiceProvider serviceProvider,
            ISagaStateRepository stateRepository)
        {
            _logger = logger;
            _orderServiceClient = orderServiceClient;
            _inventoryServiceClient = inventoryServiceClient;
            _paymentServiceClient = paymentServiceClient;
            _serviceProvider = serviceProvider;
            this.stateRepository = stateRepository;
        }
        /// <summary>
        /// Xủ lý các bước tạm dừng hoặc lỗi
        /// </summary>
        public async Task ResumeFailSagaAsync(CancellationToken cancellationToken = default)
        {
            var pendingSagas = await stateRepository.GetPendingSagasAsync(cancellationToken);
            foreach (var saga in pendingSagas)
            {
                _logger.LogInformation("Resuming failed saga: {OrderId}, current step: {Step}", saga.OrderId, saga.CurrentStep);
            }
        }

        public async Task StartOrderProcessingSaga(OrderCreatedIntegrationEvent orderEvent, CancellationToken cancellationToken = default)
        {
            // check đơn hàng đã được lưu chưa
            var existingSaga = await stateRepository.GetSagaStateAsync(orderEvent.OrderId, cancellationToken);
            if (existingSaga != null)
            {
                _logger.LogWarning("Saga đã tồn tại cho OrderId: {OrderId}. Bỏ qua việc tạo mới.", orderEvent.OrderId);
                return;
            }

            // tạo saga cho đơn hàng mới
            var newSagaState = new OrderSagaState
            {
                OrderId = orderEvent.OrderId,
                UserId = orderEvent.UserId,
                TotalAmount = orderEvent.TotalPrice,
                StartedAt = DateTime.UtcNow,
                Status = Enums.SagaStatus.Started
            };
            await stateRepository.SaveSagaStateAsync(newSagaState, cancellationToken);

            try {
                //b1: đánh dấu đơn đang xử lý
                await _orderServiceClient.MarkOrderAsProcessingAsync(orderEvent.OrderId, cancellationToken: cancellationToken);
                newSagaState.CurrentStep = OrderSagaStep.OrderMarkedAsProcessing;
                await stateRepository.UpdateSagaStateAsync(newSagaState, cancellationToken);


                //b2: kiểm tra kho hàng, cập nhật số lượng đặt hàng
                // lấy danh sách sản phâm trong đơn hàng
                var orderItem = await _orderServiceClient.GetOrderItemByIdAsync(orderEvent.OrderId);
                //kiểm tra tồn kho của mỗi sản phẩm trong đơn hàng
                if (orderItem != null) {
                    foreach (var item in orderItem)
                    {
                        // trả về true/false đặt hàng => giảm số lượng trong db để giữ hàng tránh bị xung đột
                        var inventoryReserve = await _inventoryServiceClient.ReserveInventoryAsync(item.ProductId, item.Quantity, cancellationToken);
                        Console.WriteLine("Checkkkkkk reserver order");
                        if (!inventoryReserve)
                        {
                            // false => không đủ hàng
                            await CompensateAndCancelOrderAsync(newSagaState, "Không đủ hàng trong kho", cancellationToken);
                            return;
                        }
                        // true => đủ hàng, lưu danh sách sản phẩm cần giữ hàng
                        newSagaState.ReservedItems.Add(item); // thêm vào csdl inventory

                        newSagaState.CurrentStep = OrderSagaStep.InventoryReserved; // đánh dấu bước mà saga đã làm tới
                        await stateRepository.UpdateSagaStateAsync(newSagaState, cancellationToken);

                        // Thêm delay để test DB
                        //Console.WriteLine($"Đã reserve {item.Quantity} sản phẩm {item.ProductId}. Chờ 5 giây...");
                        //await Task.Delay(7000, cancellationToken); // 7 giây
                    }
                }
                //b3: thực hiện thanh toán - payment service 
                // tạo thanh toán
                var (isPaymentCreated, paymentId, message) =
                    await _paymentServiceClient.CreatePaymentAsync(orderEvent.OrderId, orderEvent.TotalPrice, cancellationToken);

                if (!isPaymentCreated) {
                    await CompensateAndCancelOrderAsync(newSagaState, "Tạo giao dịch thanh toán thất bại", cancellationToken);
                    return;
                }
                // đánh dấu thanh toán đang được xử lý
                var paymentProcessed = await _paymentServiceClient.ProcessPaymentAsync(paymentId, cancellationToken);

                if (!paymentProcessed)
                {
                    _logger.LogWarning("Payment processing failed for OrderId: {OrderId}", orderEvent.OrderId);
                    await _paymentServiceClient.MarkPaymentFailedAsync(paymentId, cancellationToken);
                    await CompensateAndCancelOrderAsync(newSagaState, "Thanh toán thất bại", cancellationToken);
                    return;
                }

                newSagaState.CurrentStep = OrderSagaStep.PaymentProcessed;
                await stateRepository.UpdateSagaStateAsync(newSagaState, cancellationToken);


                //b4: đánh dấu đơn hàng hoàn thành - order service gọi api đến báo hoàn thành đơn hàng
                //-> đánh dấu đơn hàng đã thanh toán nếu payment trả success

                // cập nhật lại số lượng giữ hàng trong kho
                // Bước thanh toán thành công xong
                foreach (var item in newSagaState.ReservedItems)
                {
                    await _inventoryServiceClient.ConfirmInventoryAsync( item.ProductId );
                }

                await _orderServiceClient.MarkOrderAsPaidAsync(orderEvent.OrderId, cancellationToken);
                newSagaState.CurrentStep = OrderSagaStep.OrderCompleted;
                newSagaState.Status = SagaStatus.Completed;
                await stateRepository.UpdateSagaStateAsync(newSagaState, cancellationToken);

                // mở rộng gửi thông báo cho người dùng
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing order: {OrderId}", orderEvent.OrderId);
                await CompensateAndCancelOrderAsync(newSagaState, ex.Message, cancellationToken);
            }

        }

        private async Task CompensateAndCancelOrderAsync(OrderSagaState sagaState, string reason, CancellationToken cancellationToken = default)
        {
            try
            {
                switch (sagaState.CurrentStep)
                {
                    // Xử lý hoàn tiền cho đơn hàng thanh toán thất bại.
                    case OrderSagaStep.PaymentProcessed:
                        Console.WriteLine("check Xử lý hoàn tiền cho đơn hàng thanh toán thất bại.");
                        var refundResult = await _paymentServiceClient.RequestRefundAsync(
                            sagaState.OrderId,
                            sagaState.TotalAmount,
                            cancellationToken
                        );
                        goto case OrderSagaStep.InventoryReserved;

                    // Xử lý số lượng sản phẩm đã giữ tạm
                    case OrderSagaStep.InventoryReserved:
                        Console.WriteLine("check Xử lý số lượng sản phẩm đã giữ tạm");
                        var orderItem = await _orderServiceClient.GetOrderItemByIdAsync(sagaState.OrderId);
                        if (sagaState.ReservedItems != null && sagaState.ReservedItems.Any())
                        {
                            foreach (var item in sagaState.ReservedItems)
                            {
                                _logger.LogInformation(
                                    "Releasing inventory: ProductId={ProductId}, Quantity={Quantity}",
                                    item.ProductId,
                                    item.Quantity
                                );

                                await _inventoryServiceClient.ReleaseInventoryAsync(
                                    item.ProductId,
                                    item.Quantity,
                                    cancellationToken
                                );
                            }
                        }
                        goto case OrderSagaStep.OrderMarkedAsProcessing;

                    // Cập nhật đơn hàng thất bại trong OrderService
                    case OrderSagaStep.OrderMarkedAsProcessing:
                        await _orderServiceClient.MarkOrderAsCancelledAsync(sagaState.OrderId, reason, cancellationToken);
                        break;

                }
                // cập nhật trạng thái saga là thất bại
                sagaState.Status = SagaStatus.Failed;
                sagaState.FailureReason = reason;
                sagaState.CompletedAt = DateTime.Now;
                await stateRepository.UpdateSagaStateAsync(sagaState, cancellationToken);

                // gửi thông báo cho người dùng nếu cần
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during compensation for order: {OrderId}", sagaState.OrderId);
                // Lưu lỗi để xử lý thủ công sau
                sagaState.Status = SagaStatus.CompensationFailed;
                sagaState.FailureReason = $"Lỗi hoàn tiền: {ex.Message}";
                await stateRepository.UpdateSagaStateAsync(sagaState, cancellationToken);
            }
        }

    }
}


//+----------------+ +------------------+ +----------------+ +-----------------------+
//| User / API | -----> | Order Service | -----> | Message Broker | -----> | OrderSaga Consumer |
//| Create Order |        | Create Order |        | (RabbitMQ) |        | (OrderCreatedEvent) |
//+----------------+ +------------------+ +----------------+ +-----------------------+
//       |                           |                          |                          |
//       |                           |                          |                          |
//       |                           |                  Save order as Pending |                           
//       |                           |                   Publish OrderCreated |                       
//       |                           |                      Event |                          |
//       |                           | ------------------------->|                          |
//       |                           |                          |                          |
//       |                           |                          |                          |
//       |                           |                          v                           |
//       |                           |                  +----------------+                  |
//       |                           |                 | SagaRepo |                  |
//       |                           |                 | Save saga state|                  |
//       |                           |                 +----------------+                  |
//       |                           |                          |                          |
//       |                           |                          v                          |
//       |                           |                 +----------------------+           |
//       |                           |                 |   Orchestrator       |           |
//       |                           |                 | (Saga Steps Logic)   |           |
//       |                           |                 +----------------------+           |
//       |                           |                          |                          |
//       |                           |                          | Step 1: Mark Order Processing
//       |                           |                          v
//       |                           |                 +----------------+
//       |                           |                 | OrderService   |
//       |                           |                 | Mark as Processing
//       |                           |                 +----------------+
//       |                           |                          |
//       |                           | Step 2: Reserve Inventory
//       |                           |                          v
//       |                           |                 +----------------+
//       |                           |                 | InventoryService|
//       |                           |                 | Reserve Items  |
//       |                           |                 +----------------+
//       |                           |                          |
//       |                           | Step 3: Process Payment
//       |                           |                          v
//       |                           |                 +----------------+
//       |                           |                 | PaymentService |
//       |                           |                 | Process Payment|
//       |                           |                 +----------------+
//       |                           |                          |
//       |                           | Step 4: Mark Order Completed
//       |                           |                          v
//       |                           |                 +----------------+
//       |                           |                 | OrderService   |
//       |                           |                 | Mark as Paid   |
//       |                           |                 +----------------+
//       |                           |                          |
//       |                           | Step 5: Notify Customer
//       |                           |                          v
//       |                           |                 +----------------+
//       |                           |                 | Notification   |
//       |                           |                 | Service        |
//       |                           |                 +----------------+
//       |                           |
//       | If any step fails, go to Compensation Logic:
//       | v
//       | +------------------------+
//       |                 | CompensateAndCancel |
//       |                 | -Refund payment |
//       |                 | -Release inventory |
//       |                 | -Cancel order |
//       | +------------------------+


