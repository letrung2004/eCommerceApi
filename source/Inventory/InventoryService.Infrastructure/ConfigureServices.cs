using InventoryService.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrarySolution.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure
{
    public static class ConfigureServices
    {
        /// <summary>
        /// Đăng ký DbContext và các service liên quan đến database
        /// Không cấu hình JWT ở đây nữa
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 🔹 Kết nối database Identity
            var connectionString = configuration.GetConnectionString("InventoryConnection");

            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sql => sql
                        .MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName)
                        .MigrationsHistoryTable("__EFMigrationsHistory_Inventory")
                )
            );

            // Nếu muốn đăng ký repository thủ công, có thể thêm ở đây

            return services;
        }
    }
}
