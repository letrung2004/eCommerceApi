using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Application;
using OrderService.Application.Services.Implementations;
using OrderService.Application.Services.Interfaces;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Data;
using SharedLibrarySolution.Configuration;
using SharedLibrarySolution.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("MassTransit", LogLevel.Debug);
builder.Logging.AddFilter("MassTransit.RabbitMqTransport", LogLevel.Debug);

// gRPC client
builder.Services.AddGrpcClient<InventoryService.gRPC.Inventory.InventoryClient>(client =>
{
    client.Address = new Uri("http://inventory-service:8083"); // docker-compose
    //client.Address = new Uri("http://localhost:8083");
});
builder.Services.AddScoped<IInventoryServiceClient, InventoryServiceClient>();

// PRODUCER 
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        // Đọc từ appsettings.json
        var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitMqUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitMqPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(rabbitMqHost, h =>
        {
            h.Username(rabbitMqUser);
            h.Password(rabbitMqPass);
        });

    });
});

// Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJWTAuthenticationScheme(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddSwaggerDocumentation(
    serviceName: "Order Service",
    description: "Order API for the E-Commerce system",
    contactName: "Order Service API Team",
    contactEmail: "support@ecommerceapi.com"
);

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterAssemblyTypes(typeof(OrderService.Infrastructure.ConfigureServices).Assembly)
        .Where(t => t.Name.EndsWith("Repository") ||
                    t.Name.EndsWith("Service") ||
                    t.Name.EndsWith("Hasher") ||
                    t.Name.EndsWith("UnitOfWork"))
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
});

var app = builder.Build();

// Tự động migrate OrderDbContext khi container chạy
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>(); // Thay OrderDbContext bằng tên DbContext của bạn
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Starting database migration...");
        dbContext.Database.Migrate();
        logger.LogInformation("Database migration completed successfully");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
        throw;
    }
}

app.UseSharedPolicies();
app.UseSwaggerDocumentation("order");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();