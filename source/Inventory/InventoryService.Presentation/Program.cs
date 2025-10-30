using Autofac;
using Autofac.Extensions.DependencyInjection;
using InventoryService.Application;
using InventoryService.Infrastructure;
using SharedLibrarySolution.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

//auto DI
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// cấu hình jwt để đăng nhập
//builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// cấu hình kết nói SQL server
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(); // cấu hình MediatR, AutoMapper hoặc Validator.
builder.Services.AddJWTAuthenticationScheme(builder.Configuration); // cấu hình Cấu hình middleware xác thực.


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Autofac Container
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterAssemblyTypes(typeof(InventoryService.Infrastructure.ConfigureServices).Assembly)
        .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Service") || t.Name.EndsWith("Hasher"))
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
