using Microsoft.AspNetCore.Http;
using SharedLibrarySolution.Exceptions;
using SharedLibrarySolution.Logs;
using SharedLibrarySolution.Responses;
using System.Net;
using System.Text.Json;

namespace SharedLibrarySolution.Middleware
{
    public class GlobalException
    {
        private readonly RequestDelegate _next;

        public GlobalException(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                //Nếu response chưa bắt đầu và là các lỗi thông dụng
                if (!context.Response.HasStarted)
                {
                    switch (context.Response.StatusCode)
                    {
                        case StatusCodes.Status401Unauthorized:
                            await WriteResponse(context, 401, "Unauthorized - Authentication required or invalid token.");
                            break;

                        case StatusCodes.Status403Forbidden:
                            await WriteResponse(context, 403, "Forbidden - You do not have permission to access this resource.");
                            break;

                        case StatusCodes.Status404NotFound:
                            await WriteResponse(context, 404, "The requested resource could not be found.");
                            break;

                        case StatusCodes.Status429TooManyRequests:
                            await WriteResponse(context, 429, "Too many requests!");
                            break;
                    }
                }
            }
            catch (AppException appEx)
            {
                // ⚠️ Xử lý riêng AppException
                LogException.LogError(appEx);
                await HandleAppExceptionAsync(context, appEx);
            }
            catch (Exception ex)
            {
                // ❌ Xử lý các lỗi không xác định
                LogException.LogError(ex);
                await HandleUnknownExceptionAsync(context, ex);
            }
        }

        private static async Task HandleAppExceptionAsync(HttpContext context, AppException exception)
        {
            context.Response.StatusCode = exception.StatusCode;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>(
                code: exception.StatusCode,
                message: exception.Message
            );

            await context.Response.WriteAsync(JsonSerializer.Serialize(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        private static async Task HandleUnknownExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>(
                code: 500,
                message: "Internal Server Error"
            );

            await context.Response.WriteAsync(JsonSerializer.Serialize(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        private static async Task WriteResponse(HttpContext context, int statusCode, string message)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>(
                code: statusCode,
                message: message
            );

            var json = JsonSerializer.Serialize(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(json);
        }
    }
}
