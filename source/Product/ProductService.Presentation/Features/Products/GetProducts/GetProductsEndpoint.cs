using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Products.GetProducts
{
    public static class GetProductsEndpoint
    {
        public static void MapGetProductsEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/products", async (
                [AsParameters] GetProductsQuery query,
                GetProductsHandler handler) =>
            {
                var response = await handler.HandleAsync(query);

                return Results.Ok(new ApiResponse<PageResponse<ProductsResponse>>(StatusCodes.Status200OK, response));
            })
            .WithTags("Products")

            .Produces<ApiResponse<PageResponse<ProductsResponse>>>(StatusCodes.Status200OK);
        }
    }
}