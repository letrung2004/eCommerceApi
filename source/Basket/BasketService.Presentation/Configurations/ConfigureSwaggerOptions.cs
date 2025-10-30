using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BasketService.Presentation.Configurations
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            // 1 doc duy nhất, không có version
            options.SwaggerDoc("default", new OpenApiInfo
            {
                Title = "Baskets Service API",
                Description = "Authentication and Authorization API for the E-Commerce system",
                Contact = new OpenApiContact
                {
                    Name = "Baskets Service API Team",
                    Email = "support@ecommerceapi.com"
                }
            });

            // JWT Bearer setup
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {token}'"
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
        }
    }
}
