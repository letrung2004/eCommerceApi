//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using OrderService.Infrastructure.Data;

//namespace OrderService.Infrastructure
//{
//    public static class ConfigureServices
//    {
//        /// <summary>
//        /// Đăng ký DbContext và các service liên quan đến database
//        /// Không cấu hình JWT ở đây nữa
//        /// </summary>
//        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
//        {
//            // 🔹 Kết nối database Identity
//            var connectionString = configuration.GetConnectionString("OrderConnection");

//            services.AddDbContext<OrderDbContext>(options =>
//                options.UseSqlServer(
//                    connectionString,
//                    sql => sql
//                        .MigrationsAssembly(typeof(OrderDbContext).Assembly.FullName)
//                        .MigrationsHistoryTable("__EFMigrationsHistory_OrderDB")
//                )
//            );

//            // Nếu muốn đăng ký repository thủ công, có thể thêm ở đây

//            return services;
//        }
//    }
//}
