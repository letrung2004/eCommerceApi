using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Reviews.GetReviews
{
    public static class GetReviewsEndpoint
    {
        public static void MapGetReviewsEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/reviews", async (
                [AsParameters] GetReviewsQuery query,
                GetReviewsHandler handler) =>
            {
                var response = await handler.HandleAsync(query);
                return Results.Ok(response);
            })
            .WithTags("Reviews")
            .Produces<ApiResponse<PageResponse<ReviewResponse>>>(StatusCodes.Status200OK);
        }
    }
}
