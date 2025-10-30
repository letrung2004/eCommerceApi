using Microsoft.AspNetCore.Mvc;
using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Products.DeleteProduct
{
    public static class DeleteProductEndpoint
    {
        public static void MapDeleteProductEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapDelete("/products/{id}", async (
                 [FromRoute] string id,
                 DeleteProductHandler handler
             ) =>
            {

                var response = await handler.HandleAsync(new DeleteProductRequest { Id = id });

                if (!response)
                    return Results.NotFound(new ApiResponse<string>(StatusCodes.Status404NotFound, $"Product with id {id} not found."));

                return Results.Ok(new ApiResponse<string>(StatusCodes.Status200OK, "Success", $"Product {id} deleted successfully."));
            })
            .RequireAuthorization("RequireAdminOrSeller")
            .WithTags("Products")
            .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<string>>(StatusCodes.Status404NotFound)
            .DisableAntiforgery(); // để test Postman
        }
    }
}
