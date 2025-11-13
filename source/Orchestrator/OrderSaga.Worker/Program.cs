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
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Debug); // Hiển thị logs MassTransit
    })
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
            x.AddConsumer<OrderCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host($"rabbitmq://{rabbitMqSettings!.Host}", h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                // Queue và consumer
                cfg.ReceiveEndpoint("order_saga_queue", e =>
                {
                    e.ConfigureConsumer<OrderCreatedConsumer>(context);
                });

                // Tự tạo mọi endpoint & binding cho consumer
                cfg.ConfigureEndpoints(context);
            });
        });

        // DI Orchestrator & Repository
        services.AddScoped<ISagaOrchestrator, OrderSagaOrchestrator>();
        services.AddScoped<ISagaStateRepository, SagaStateRepository>();

        // Service clients
        services.AddScoped<IInventoryServiceClient, InventoryServiceClient>();
        services.AddScoped<IPaymentServiceClient, PaymentServiceClient>();
        services.AddScoped<IOrderServiceClient, OrderServiceClient>();

        // gRPC clients
        services.AddGrpcClient<InventoryService.gRPC.Inventory.InventoryClient>(options =>
            options.Address = new Uri(configuration["ServiceUrls:InventoryService"]!));

        services.AddGrpcClient<PaymentService.gRPC.PaymentService.PaymentServiceClient>(options =>
            options.Address = new Uri(configuration["ServiceUrls:PaymentService"]!));

        services.AddGrpcClient<PaymentService.gRPC.RefundService.RefundServiceClient>(options =>
            options.Address = new Uri(configuration["ServiceUrls:PaymentService"]!));

        // HTTP client for OrderService
        services.AddHttpClient<IOrderServiceClient, OrderServiceClient>(client =>
            client.BaseAddress = new Uri(configuration["ServiceUrls:OrderService"]!));
    })
    .Build();

// Migrate database nếu chưa có
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<SagaDbContext>();
    dbContext.Database.Migrate();
}

Console.WriteLine("OrderSaga.Worker started. Listening for events...");
await host.RunAsync();
