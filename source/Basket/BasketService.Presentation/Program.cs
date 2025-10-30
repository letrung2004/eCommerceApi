using BasketService.Presentation.Configurations;
using BasketService.Presentation.Features.Basket.GetBasket;
using BasketService.Presentation.Features.Baskets.AddItemToBasket;
using BasketService.Presentation.Features.Baskets.Consumers;
using BasketService.Presentation.Features.Baskets.GetBasket;
using MassTransit;
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
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
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
builder.Services.AddSwaggerDocumentation();

// cấu hình redis
builder.Services.AddRedisConfiguration(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

//middle ware
app.UseSharedPolicies();


// 🔹 Swagger
app.UseSwaggerDocumentation();
// Chứng thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();

// Basket Endpoints
app.MapAddItemEndpoint();
app.MapGetBasketEndpoint();



app.Run();
