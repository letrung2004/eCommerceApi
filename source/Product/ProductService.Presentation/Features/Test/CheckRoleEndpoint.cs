using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProductService.Presentation.Features.Test
{
    public static class CheckRoleEndpoint
    {
        public static void MapCheckRoleEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/products/test-role", [Authorize] (HttpContext context) =>
            {
                var user = context.User;

                if (user?.Identity?.IsAuthenticated != true)
                    return Results.Unauthorized();

                var roles = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                return Results.Ok(new
                {
                    Message = "User roles fetched successfully",
                    Roles = roles
                });
            });
        }
    }
}
