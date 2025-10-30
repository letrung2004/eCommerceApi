using FluentValidation;
using MassTransit;
using ProductService.Presentation.Configurations;
using ProductService.Presentation.Data;
using ProductService.Presentation.Entities.Events;
using ProductService.Presentation.Features.Categories.GetCategories;
using ProductService.Presentation.Features.Products.CreateProduct;
using ProductService.Presentation.Features.Products.DeleteProduct;
using ProductService.Presentation.Features.Products.GetProductById;
using ProductService.Presentation.Features.Products.GetProducts;
using ProductService.Presentation.Features.Products.UpdateProduct;
using ProductService.Presentation.Features.Promotions.GetPromotions;
using ProductService.Presentation.Features.Reviews.GetReviews;
using ProductService.Presentation.Features.Test;
using ProductService.Presentation.Services;
using SharedLibrarySolution.DependencyInjection;
using SharedLibrarySolution.Mapping;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJWTAuthenticationScheme(builder.Configuration); // lấy secrect key để decode
builder.Services.AddHttpContextAccessor();

// PRODUCER - Cấu hình MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Cấu hình Publish cho ProductUpdatedEvent
        cfg.Message<ProductUpdatedEvent>(m => m.SetEntityName("product_exchange"));
        cfg.Publish<ProductUpdatedEvent>(p => p.ExchangeType = "direct");
    });
});

// Đăng ký MongoDB Context
builder.Services.AddSingleton<MongoDbContext>();

// Đăng ký Handler
builder.Services.AddScoped<GetProductsHandler>();
builder.Services.AddScoped<GetCategoriesHandler>();
builder.Services.AddScoped<GetPromotionsHandler>();
builder.Services.AddScoped<GetReviewsHandler>();
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<DeleteProductHandler>();
builder.Services.AddScoped<UpdateProductHandler>();
builder.Services.AddScoped<GetProductByIdHandler>();


//Khai báo AutoMapper, tìm MappingProfile trong Assembly(dự án này)
//builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// Đăng ký tất cả validator
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CloudinaryService>(); // service upload ảnh



var app = builder.Build();


// Khởi tạo MongoDB (tạo DB, collections, indexes)
using (var scope = app.Services.CreateScope())
{
    var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    await MongoDbInitializer.InitializeAsync(mongoContext);
}


// 🔹 Global Exception Middleware
//app.UseSharedPoliciesForBackendServices(); // vừa có GlobalException vừa có chặn các request với header k phải gateway
app.UseSharedPolicies();


// 🔹 Swagger
app.UseSwaggerDocumentation();

// Chứng thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();


// Map Endpoints
app.MapGetProductsEndpoint();
app.MapGetCategoriesEndpoint();
app.MapGetPromotionsEndpoint();
app.MapGetReviewsEndpoint();
app.MapCreateProductEndpoint();
app.MapDeleteProductEndpoint();
app.MapUpdateProductEndpoint();
app.MapGetProductByIdEndpoint();
//check role endpoint
app.MapCheckRoleEndpoint();



app.Run();
