using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityService.Application;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Security;
using IdentityService.Infrastructure.Data;
using SharedLibrarySolution.Configuration;
using SharedLibrarySolution.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dùng Autofac để DI tự động
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Cấu hình JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Đăng ký các tầng
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJWTAuthenticationScheme(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// Swagger - Shared Library
builder.Services.AddSwaggerDocumentation(
    serviceName: "Identity Service",
    description: "Authentication and Authorization API for the E-Commerce system",
    contactName: "Identity Service API Team",
    contactEmail: "support@ecommerceapi.com"
);

// Autofac Container
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterAssemblyTypes(typeof(IdentityService.Infrastructure.ConfigureServices).Assembly)
        .Where(t => t.Name.EndsWith("Repository") ||
                    t.Name.EndsWith("Service") ||
                    t.Name.EndsWith("Hasher"))
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
});

var app = builder.Build();

// Global Exception Middleware
app.UseSharedPolicies();

// Swagger UI
app.UseSwaggerDocumentation("identity");

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Auto migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    db.Database.Migrate();
}

app.Run();