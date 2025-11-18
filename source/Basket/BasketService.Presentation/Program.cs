using BasketService.Presentation.Configurations;
using BasketService.Presentation.Features.Basket.GetBasket;
using BasketService.Presentation.Features.Baskets.AddItemToBasket;
using BasketService.Presentation.Features.Baskets.Consumers;
using BasketService.Presentation.Features.Baskets.GetBasket;
using MassTransit;
using SharedLibrarySolution.Configuration;
using SharedLibrarySolution.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//JWT Authentication
builder.Services.AddJWTAuthenticationScheme(builder.Configuration);
builder.Services.AddHttpContextAccessor();

// CONSUMER - Cấu hình MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductUpdatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitMqUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitMqPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(rabbitMqHost, h =>
        {
            h.Username(rabbitMqUser);
            h.Password(rabbitMqPass);
        });

        cfg.ReceiveEndpoint("basket_product_update_queue", e =>
        {
            e.ConfigureConsumeTopology = false; // tắc bidding mặc định

            // Bind tới exchange product_exchange, nhận tất cả routing key
            e.Bind("product_exchange", s =>
            {
                s.ExchangeType = "direct";
                s.RoutingKey = "product.updated";
            });
            e.ConfigureConsumer<ProductUpdatedConsumer>(context);
        });
    });
});




// Add services to the container.
//Khai báo AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Basket Handlers
builder.Services.AddScoped<AddItemHandler>();
builder.Services.AddScoped<GetBasketHandler>();


builder.Services.AddControllers();
// ✅ Swagger - Shared Library
builder.Services.AddSwaggerDocumentation(
    serviceName: "Basket Service",
    description: "Basket API document",
    contactName: "Basket Service API Team",
    contactEmail: "support@ecommerceapi.com"
);
// cấu hình redis
builder.Services.AddRedisConfiguration(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

//middle ware
app.UseSharedPolicies();


// Swagger
app.UseSwaggerDocumentation("basket");
// Chứng thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();

// Basket Endpoints
app.MapAddItemEndpoint();
app.MapGetBasketEndpoint();



app.Run();
