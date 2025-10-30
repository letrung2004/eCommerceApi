using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedLibrarySolution.Logs;
using SharedLibrarySolution.Middleware;

namespace SharedLibrarySolution.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services,
                    IConfiguration configuration,
                    string fileName) where TContext : DbContext
        {
            // serilog config
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.File($"{fileName}_logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // logging to service container
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true)
            );

            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, configuration);

            return services;
        }
        /// <summary>
        /// Middleware: log request/response + exception + CID + elapsed time
        /// </summary>
        public static IApplicationBuilder UseSharedLogging(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await LogException.HandleRequestAsync(context, next);
            });
            return app;
        }

        /// <summary>
        /// Sử dụng cho API Gateway - CHỈ có GlobalException
        /// </summary>
        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();
            return app;
        }

        /// <summary>
        /// Sử dụng cho Backend Services - có cả GlobalException và ListenToOnlyApiGateway
        /// </summary>
        public static IApplicationBuilder UseSharedPoliciesForBackendServices(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();
            app.UseMiddleware<ListenToOnlyApiGateway>();
            return app;
        }
    }
}