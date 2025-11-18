using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharedLibrarySolution.Configuration
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly string _serviceName;
        private readonly string _description;
        private readonly string _contactName;
        private readonly string _contactEmail;

        public ConfigureSwaggerOptions(string serviceName, string description, string contactName, string contactEmail)
        {
            _serviceName = serviceName;
            _description = description;
            _contactName = contactName;
            _contactEmail = contactEmail;
        }

        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc("default", new OpenApiInfo
            {
                Title = $"{_serviceName} API",
                Description = _description,
                Contact = new OpenApiContact
                {
                    Name = _contactName,
                    Email = _contactEmail
                }
            });

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
