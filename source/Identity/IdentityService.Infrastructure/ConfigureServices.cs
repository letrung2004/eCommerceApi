using IdentityService.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrarySolution.DependencyInjection;

namespace IdentityService.Infrastructure
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
            var connectionString = configuration.GetConnectionString("IdentityConnection");

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sql => sql
                        .MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)
                        .MigrationsHistoryTable("__EFMigrationsHistory_Identity")
                )
            );

            // 🔹 Nếu muốn đăng ký repository thủ công, có thể thêm ở đây
            // services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        /// <summary>
        /// Middleware chung cho backend service (nếu muốn)
        /// Ví dụ: ListenToOnlyApiGateway, logging,...
        /// </summary>
        public static IApplicationBuilder UseInfrastructurePolicies(this IApplicationBuilder app)
        {
            SharedServiceContainer.UseSharedPoliciesForBackendServices(app);
            return app;
        }
    }
}
