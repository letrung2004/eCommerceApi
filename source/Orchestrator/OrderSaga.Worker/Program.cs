using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderSaga.Worker.Consumers;
using OrderSaga.Worker.Data;
using OrderSaga.Worker.Orchestrator.Implementations;
using OrderSaga.Worker.Orchestrator.Interfaces;
using OrderSaga.Worker.Repositories.Implementations;
using OrderSaga.Worker.Repositories.Interfaces;
using OrderSaga.Worker.Services.Implementations;
using OrderSaga.Worker.Services.Interfaces;
using OrderSaga.Worker.Settings;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        // DbContext
        services.AddDbContext<SagaDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SagaStateDb")));

        // RabbitMQ settings
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));
        var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();

        // MassTransit
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>(); // đăng ký consummer xử lý event
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri($"rabbitmq://{rabbitMqSettings!.Host}"), h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                // đăng ký nhận event OrderCreatedIntegrationEvent do OrderCreatedConsumer kế thừa OrderCreatedIntegrationEvent
                cfg.ReceiveEndpoint("order_saga_queue", e => // ở order service kh cần gửi Chỉ cần publish đúng loại event → RabbitMQ sẽ định tuyến đúng nhờ binding mà MassTransit tự tạo.
                {
                    e.ConfigureConsumer<OrderCreatedConsumer>(context); // gắn consumer vào queue
                });
            });
        });

        // đăng ký DI Orchestrator & Repository
        services.AddScoped<ISagaOrchestrator, OrderSagaOrchestrator>();
        services.AddScoped<ISagaStateRepository, SagaStateRepository>();

        // gRPC client cho InventoryService
        services.AddGrpcClient<InventoryService.gRPC.Inventory.InventoryClient>(options =>
        {
            options.Address = new Uri(configuration["ServiceUrls:InventoryService"]!);
        });

        // gRPC client cho PaymentService -- check chỗ này chưa import dc
        services.AddGrpcClient<PaymentService.gRPC.PaymentService.PaymentServiceClient>(options =>
        {
            options.Address = new Uri(configuration["ServiceUrls:PaymentService"]!);
        });

        // HTTP client for OrderService
        services.AddHttpClient<IOrderServiceClient, OrderServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ServiceUrls:OrderService"]!);
        });
    })
    .Build();

// Tạo database nếu chưa tồn tại
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<SagaDbContext>();
    dbContext.Database.Migrate();
}

await host.RunAsync();  // ở đây host sẽ tạo 1 CancellationToken ngầm



// đây là background worker lắng nghe event trên message broker (RabbitMQ / MassTransit)
// để thực hiện các điều phối xử lý logics đến nhiều service liên quan đến nhau



//Consumers/	Chứa các lớp Consumer (trình tiêu thụ event) — đây là điểm vào chính của Saga, nơi nhận event từ RabbitMQ (qua MassTransit). Ví dụ: OrderCreatedConsumer.cs sẽ nhận event OrderCreatedEvent từ OrderService.

//Orchestrator/	Chứa lớp trung tâm như OrderSagaOrchestrator.cs, nơi điều phối toàn bộ các bước của Saga: gọi gRPC, cập nhật trạng thái, publish event tiếp theo.

//Repositories/	Chứa các lớp thao tác DB (CRUD) như OrderSagaRepository.cs giúp lưu/cập nhật OrderSagaState.

//Services/	Chứa các client để gọi tới các service khác qua gRPC, ví dụ: InventoryGrpcClient, PaymentGrpcClient, RefundGrpcClient.

//Settings/	Chứa cấu hình app, như appsettings.json + các class config cho MassTransit, RabbitMQ, DB, gRPC endpoints.

// sự kiện order.created được gửi lên => ở work ordersaga có OrderCreateConsummer hứng event sau đó gọi đến
// orchestrator để xử lý nghiệp vụ consummer chị lắng nghe khong xử lý các logic => sau đó gửi event 

//[User]
//   ↓ HTTP
//OrderService
//   ├─> gRPC → Inventory.CheckStock
//   ├─> Save Order (Pending)
//   └─> Publish → OrderCreatedEvent
//        ↓
//[OrderSaga.Worker]
//   ├─> Consumer nhận event
//   ├─> SaveSagaStateAsync (step=None, status = InProgress)
//   └─> Orchestrator xử lý tuần tự:
//        1.ReserveInventory → publish InventoryReservedEvent
//        2. PreAuthorizePayment → publish PaymentAuthorizedEvent
//        3. CapturePayment → publish OrderConfirmedEvent
//        ⚠️ Nếu fail bất kỳ bước nào:
//            → rollback / release inventory
//            → update saga state: Failed
//            → publish compensation events: OrderCancelledEvent / PaymentFailedEvent
