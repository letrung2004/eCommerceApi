using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Products.GetProductById
{
    public static class GetProductByIdEndpoint
    {
        public static void MapGetProductByIdEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/products/{id}", async (
                string id,
                GetProductByIdHandler handler) =>
            {
                var product = await handler.HandleAsync(id);
                if (product == null)
                    return Results.NotFound(new ApiResponse<string>(StatusCodes.Status404NotFound,"Product not found"));

                return Results.Ok(new ApiResponse<ProductsResponse>(StatusCodes.Status200OK, product));
            })
            .WithTags("Products")
            .Produces<ApiResponse<ProductsResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<string>>(StatusCodes.Status404NotFound);
        }
    }
}