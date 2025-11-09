using Autofac;
using Autofac.Extensions.DependencyInjection;
using InventoryService.Application;
using OrderService.Infrastructure;
using OrderService.Presentation.Configuration;
using SharedLibrarySolution.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


// Dùng Autofac để DI tự động không cần khai báo
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());


// cấu hình kết nói SQL server
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(); // cấu hình MediatR, AutoMapper hoặc Validator.
builder.Services.AddJWTAuthenticationScheme(builder.Configuration); // cấu hình Cấu hình middleware xác thực.



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()); // chuyển enum int về string
    });

builder.Services.AddSwaggerDocumentation();

//Autofac - auto dependency injections Container
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterAssemblyTypes(typeof(OrderService.Infrastructure.ConfigureServices).Assembly)
        .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Service") || t.Name.EndsWith("Hasher"))
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
});

var app = builder.Build();

// Global Exception Middleware
//app.UseSharedPoliciesForBackendServices(); // vừa có GlobalException vừa có chặn các request với header k phải gateway
app.UseSharedPolicies(); // test khi chưa bật gateway

// Swagger
app.UseSwaggerDocumentation();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();
