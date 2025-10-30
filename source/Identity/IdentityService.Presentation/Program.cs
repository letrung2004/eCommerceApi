using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityService.Application;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Security;
using IdentityService.Presentation.Configuration;
using SharedLibrarySolution.DependencyInjection;


var builder = WebApplication.CreateBuilder(args); // khỏi tạo đối tượng để đăng ký các DI, middleware, service container.

// Dùng Autofac để DI tự động không cần khai báo
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());


// đọc cấu hình jwt để AddJWTAuthenticationScheme sử dụng
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


// Đăng ký các tầng
builder.Services.AddInfrastructureServices(builder.Configuration); // kết nối database
builder.Services.AddApplicationServices(); // cấu hình MediatR, AutoMapper hoặc Validator.
builder.Services.AddJWTAuthenticationScheme(builder.Configuration); // cấu hình Cấu hình middleware xác thực.


builder.Services.AddControllers();// cho phép định nghĩa các controller
builder.Services.AddSwaggerDocumentation(); 

// Autofac Container
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterAssemblyTypes(typeof(IdentityService.Infrastructure.ConfigureServices).Assembly)
        .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Service") || t.Name.EndsWith("Hasher"))
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
});

var app = builder.Build(); // tạo app xong chạy qua các middleware

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
