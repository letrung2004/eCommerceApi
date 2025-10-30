using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace SharedLibrarySolution.Middleware
{
    public class ListenToOnlyApiGateway
    {
        private readonly RequestDelegate _next;
        private const string GatewayHeader = "Api-Gateway";
        private const string ExpectedHeaderValue = "my-gateway";

        // Danh sách các path được phép bypass (không cần header)
        private static readonly string[] AllowedPaths = new[]
        {
             "/swagger",
            "/identity/swagger/default/swagger.json",
        };

        public ListenToOnlyApiGateway(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Cho phép các path đặc biệt bypass
            if (ShouldBypass(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var headerValue = context.Request.Headers[GatewayHeader].FirstOrDefault();

            // Kiểm tra header từ Gateway
            if (string.IsNullOrEmpty(headerValue) || headerValue != ExpectedHeaderValue)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    statusCode = context.Response.StatusCode,
                    title = "Service Unavailable",
                    message = "This service can only be accessed through the API Gateway."
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            await _next(context);
        }

        private bool ShouldBypass(PathString path)
        {
            return AllowedPaths.Any(allowedPath =>
                path.StartsWithSegments(allowedPath, StringComparison.OrdinalIgnoreCase));
        }
    }
}