using Autofac;
using Autofac.Extensions.DependencyInjection;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Presentation.Configuration;
using SharedLibrarySolution.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// client order service  kết nối với inventory service để gọi đến các method của inventory service
builder.Services.AddGrpcClient<InventoryService.gRPC.Inventory.InventoryClient
>(client =>
{
    client.Address = new Uri("http://localhost:8083"); // môi trường dev

});

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

// note

//➡ Khi biên dịch (codegen), nó tạo ra 2 thứ:

//Bên InventoryService(server) triển khai InventoryBase
//→ trả CheckInventoryReply.

//Bên OrderService (client) sử dụng Inventory.InventoryClient
//→ gọi CheckInventoryAsync(...) và nhận về CheckInventoryReply.




//note 2

//[OrderService.Application]    <--- gọi qua gRPC --->   [InventoryService.gRPC]
//     |
//     |--- dùng IInventoryServiceClient (interface)
//     |
//     |--- return bool hoặc DTO tùy logic nội bộ
// phải có cấu hình ở file solution và cả program.cs: both( server + client), server, client
