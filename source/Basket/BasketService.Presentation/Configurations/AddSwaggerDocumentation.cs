 using Microsoft.OpenApi.Models;

namespace BasketService.Presentation.Configurations
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.ConfigureOptions<ConfigureSwaggerOptions>();
            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            // cấu hình dùng cho cả gateway và cả của identity service
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((doc, req) =>
                {
                    var isViaGateway = req.Host.Port == 8888 ||
                                       req.Headers.ContainsKey("X-Forwarded-Prefix") ||
                                       req.Headers["Referer"].ToString().Contains(":8888");

                    if (isViaGateway)
                    {
                        doc.Servers = new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Url = "http://localhost:8888/basket",
                        Description = "Via Gateway"
                    }
                };
                    }
                    else
                    {
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
                // Gọi swagger.json không có version
                options.SwaggerEndpoint("/swagger/default/swagger.json", "Basket Service API");
                options.RoutePrefix = "swagger"; // => /swagger
                options.DocumentTitle = "E-Commerce Basket API Documentation";
                options.DisplayRequestDuration();
            });

            return app;
        }
    }
}
