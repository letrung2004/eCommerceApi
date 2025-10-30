using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Promotions.GetPromotions
{
    public static class GetPromotionsEndpoint
    {
        public static void MapGetPromotionsEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/promotions", async (
                [AsParameters] GetPromotionsQuery query,
                GetPromotionsHandler handler) =>
            {
                var response = await handler.HandleAsync(query);

                return Results.Ok(response);
            })
            .WithTags("Promotions")

            .Produces<ApiResponse<PageResponse<PromotionResponse>>>(StatusCodes.Status200OK);
        }
    }
}
