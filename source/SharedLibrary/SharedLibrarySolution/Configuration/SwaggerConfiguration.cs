using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace SharedLibrarySolution.Configuration
{
    /// <summary>
    /// Swagger Configuration cho tất cả services
    /// </summary>
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Đăng ký Swagger với thông tin service
        /// </summary>
        public static IServiceCollection AddSwaggerDocumentation(
            this IServiceCollection services,
            string serviceName,
            string description = "",
            string contactName = "E-Commerce API Team",
            string contactEmail = "support@ecommerceapi.com")
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("default", new OpenApiInfo
                {
                    Title = $"{serviceName} API",
                    Description = description,
                    Contact = new OpenApiContact
                    {
                        Name = contactName,
                        Email = contactEmail
                    }
                });

                // JWT Bearer Authentication
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token. Example: 'Bearer eyJhbGc...'"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        /// <summary>
        /// Cấu hình Swagger UI với Gateway support
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="serviceName">Tên service (ví dụ: "identity", "products", "orders")</param>
        public static IApplicationBuilder UseSwaggerDocumentation(
            this IApplicationBuilder app,
            string serviceName)
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((doc, req) =>
                {
                    // Kiểm tra request có đi qua API Gateway không
                    var isViaGateway =
                        req.Headers.ContainsKey("X-Forwarded-Prefix") ||
                        req.Headers.ContainsKey("Api-Gateway") ||
                        req.Headers.ContainsKey("X-Forwarded-Host");

                    if (isViaGateway)
                    {
                        // Lấy Gateway host từ header (ưu tiên)
                        var gatewayHost = req.Headers.ContainsKey("X-Forwarded-Host")
                            ? req.Headers["X-Forwarded-Host"].ToString()
                            : DetermineGatewayHost();

                        doc.Servers = new List<OpenApiServer>
                        {
                            new OpenApiServer
                            {
                                Url = $"http://{gatewayHost}/{serviceName}",
                                Description = "Via API Gateway"
                            }
                        };
                    }
                    else
                    {
                        // Truy cập trực tiếp service (debug/development)
                        doc.Servers = new List<OpenApiServer>
                        {
                            new OpenApiServer
                            {
                                Url = $"{req.Scheme}://{req.Host.Value}",
                                Description = "Direct Access"
                            }
                        };
                    }
                });
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/default/swagger.json", $"{serviceName.ToUpper()} Service API");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = $"E-Commerce {serviceName.ToUpper()} API Documentation";
                options.DisplayRequestDuration();
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            });

            return app;
        }

        /// <summary>
        /// Xác định Gateway host dựa trên môi trường
        /// </summary>
        private static string DetermineGatewayHost()
        {
            // 1. Kiểm tra environment variable
            var gatewayUrl = Environment.GetEnvironmentVariable("GATEWAY_URL");
            if (!string.IsNullOrEmpty(gatewayUrl))
            {
                return gatewayUrl;
            }

            // 2. Dựa vào ASPNETCORE_ENVIRONMENT
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == "Production")
            {
                // Trong Docker, browser vẫn dùng localhost
                return "localhost:8888";
            }

            // 3. Default for development
            return "localhost:8888";
        }
    }
}