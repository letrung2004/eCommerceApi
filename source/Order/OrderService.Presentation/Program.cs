using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using OrderService.Application;
using OrderService.Application.Services.Implementations;
using OrderService.Application.Services.Interfaces;
using OrderService.Infrastructure;
using OrderService.Presentation.Configuration;
using SharedLibrarySolution.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("MassTransit", LogLevel.Debug);
builder.Logging.AddFilter("MassTransit.RabbitMqTransport", LogLevel.Debug);

// gRPC client
builder.Services.AddGrpcClient<InventoryService.gRPC.Inventory.InventoryClient>(client =>
{
    client.Address = new Uri("http://localhost:8083");
});
builder.Services.AddScoped<IInventoryServiceClient, InventoryServiceClient>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqSection = builder.Configuration.GetSection("RabbitMQ");
        var host = rabbitMqSection["Host"] ?? "localhost";
        var username = rabbitMqSection["Username"] ?? "guest";
        var password = rabbitMqSection["Password"] ?? "guest";

        cfg.Host(host, h =>
        {
            h.Username(username);
            h.Password(password);
        });
        cfg.ConfigureEndpoints(context);
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

builder.Services.AddSwaggerDocumentation();

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

app.UseSharedPolicies();
app.UseSwaggerDocumentation();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();