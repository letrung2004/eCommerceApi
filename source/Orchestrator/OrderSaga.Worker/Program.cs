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
        logging.SetMinimumLevel(LogLevel.Debug);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddDbContext<SagaDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SagaStateDb")));

        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));
        var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSettings!.Host, h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<ISagaOrchestrator, OrderSagaOrchestrator>();
        services.AddScoped<ISagaStateRepository, SagaStateRepository>();
        services.AddScoped<IInventoryServiceClient, InventoryServiceClient>();
        services.AddScoped<IPaymentServiceClient, PaymentServiceClient>();
        services.AddScoped<IOrderServiceClient, OrderServiceClient>();

        services.AddGrpcClient<InventoryService.gRPC.Inventory.InventoryClient>(options =>
            options.Address = new Uri(configuration["ServiceUrls:InventoryService"]!));

        services.AddGrpcClient<PaymentService.gRPC.PaymentService.PaymentServiceClient>(options =>
            options.Address = new Uri(configuration["ServiceUrls:PaymentService"]!));

        services.AddGrpcClient<PaymentService.gRPC.RefundService.RefundServiceClient>(options =>
            options.Address = new Uri(configuration["ServiceUrls:PaymentService"]!));

        services.AddHttpClient<IOrderServiceClient, OrderServiceClient>(client =>
            client.BaseAddress = new Uri(configuration["ServiceUrls:OrderService"]!));
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<SagaDbContext>();
    dbContext.Database.Migrate();
}

Console.WriteLine("✅ OrderSaga.Worker started. Listening for OrderCreatedEvent...");
await host.RunAsync();