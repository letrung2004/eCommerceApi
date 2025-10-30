using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Categories.GetCategories
{
    public static class GetCategoriesEndpoint
    {
        public static void MapGetCategoriesEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/categories", async (
                [AsParameters] GetCategoriesQuery query,
                GetCategoriesHandler handler) =>
            {
                var response = await handler.HandleAsync(query);

                return Results.Ok(response);
            })
            .WithTags("Categories") // không có cũng đc để cho swagger hiện đệp thôi

            .Produces<ApiResponse<PageResponse<CategoryResponse>>>(StatusCodes.Status200OK);
        }
    }
}
