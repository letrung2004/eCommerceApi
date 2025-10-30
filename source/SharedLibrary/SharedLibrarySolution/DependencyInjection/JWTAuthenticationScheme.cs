using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace SharedLibrarySolution.DependencyInjection
{
    public static class JWTAuthenticationScheme
    {
        // Cấu hình cần có trong appsettings.json:
        // "JwtSettings": {
        //   "SecretKey": "my_super_secret_key_123",
        //   "Issuer": "MyAuthServer",
        //   "Audience": "MyClientApp"
        // }

        public static IServiceCollection AddJWTAuthenticationScheme(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var key = Encoding.ASCII.GetBytes(secretKey!);

            // cấu hình Authentication = chứng thực
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            // Authorization = phân quyền
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole("ADMIN"));
                options.AddPolicy("RequireSellerRole", policy =>
                    policy.RequireRole("SELLER"));
                options.AddPolicy("RequireUserRole", policy =>
                    policy.RequireRole("USER"));
                options.AddPolicy("RequireAdminOrSeller", policy =>
                    policy.RequireRole("ADMIN","SELLER"));
            });

            return services;
        }
    }
}