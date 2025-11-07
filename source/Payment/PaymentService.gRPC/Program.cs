
using Microsoft.EntityFrameworkCore;
using PaymentService.gRPC.Application.Interfaces.IRepositories;
using PaymentService.gRPC.Infrastructure.Data;
using PaymentService.gRPC.Infrastructure.Repositories;
using PaymentService.gRPC.Services;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentConnection")));

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<PaymentService.gRPC.Application.Services.PaymentAppService>();
var app = builder.Build();


//Configure the HTTP request pipeline.
app.MapGrpcService<PaymentGrpcService>();
// Configure the HTTP request pipeline.
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
