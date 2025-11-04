using Autofac;
using Autofac.Extensions.DependencyInjection;
using InventoryService.Application;
using InventoryService.Infrastructure;
using InventoryService.Presentation.Configuration;
using SharedLibrarySolution.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

//auto DI
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// cấu hình kết nói SQL server
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(); // cấu hình MediatR, AutoMapper hoặc Validator.
builder.Services.AddJWTAuthenticationScheme(builder.Configuration); // cấu hình Cấu hình middleware xác thực.
builder.Services.AddQuartzJobs(); // job định kì clean các reservation bị hết hạn


// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()); // chuyển enum int về string
    });

builder.Services.AddSwaggerDocumentation();// configs swagger



// Autofac Container
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterAssemblyTypes(typeof(InventoryService.Infrastructure.ConfigureServices).Assembly)
        .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Service") || t.Name.EndsWith("Hasher"))
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
});


var app = builder.Build();

app.UseSharedPolicies(); // test khi chưa bật gateway

// Swagger
app.UseSwaggerDocumentation();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();