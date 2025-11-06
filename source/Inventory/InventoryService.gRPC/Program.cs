using InventoryService.gRPC.Domain.IRepositories;
using InventoryService.gRPC.Infrastructure.Data;
using InventoryService.gRPC.Infrastructure.Repositories;
using InventoryService.gRPC.Services;

//using InventoryService.gRPC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<InventoryServiceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InventoryConnection")));


builder.Services.AddGrpc();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<InventoryService.gRPC.Application.Services.InventoryService>();
builder.Services.AddScoped<InventoryService.gRPC.Application.Services.ProductAppService>();
var app = builder.Build();

//app.MigrateInventoryDatabase();

//Configure the HTTP request pipeline.
app.MapGrpcService<InventoryGrpcService>();
app.MapGrpcService<ProductGrpcService>(); 

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();


//InventoryService.gRPC
//├── Domain
//│   ├── InventoryItem.cs          // Model database
//│   └── IInventoryRepository.cs  // Interface repository
//├── Infrastructure
//│   ├── InventoryServiceDbContext.cs  // EF Core DbContext
//│   └── InventoryRepository.cs        // Thực thi repository
//├── Application
//│   ├── InventoryService.cs       // Business logic quản lý kho
//│   └── ProductAppService.cs      // Business logic tạo/update stock cho sản phẩm
//├── Services
//│   ├── InventoryGrpcService.cs   // gRPC server cho OrderService gọi
//│   └── ProductGrpcService.cs     // gRPC server cho ProductService gọi
//└── Protos (hoặc Share Library)
//    ├── inventory.proto           // Định nghĩa API inventory
//    └── product.proto             // Định nghĩa API product stock

//Luồng tổng quan:

//InventoryService giữ dữ liệu tồn kho.

//Business logic (Application layer) tách riêng ra để dễ test.

//Repository + DbContext làm việc với database.

//Services (InventoryGrpcService, ProductGrpcService) expose API gRPC để các service khác gọi.

//.proto định nghĩa contract giữa các service.

//InventoryBase và ProductBase được tự động sinh từ file .proto nhờ gói Grpc.Tools.