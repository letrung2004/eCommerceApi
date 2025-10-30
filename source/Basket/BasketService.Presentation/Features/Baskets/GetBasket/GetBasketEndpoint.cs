using BasketService.Presentation.Features.Baskets;
using BasketService.Presentation.Features.Baskets.GetBasket;
using SharedLibrarySolution.Responses;

namespace BasketService.Presentation.Features.Basket.GetBasket
{
    public static class GetBasketEndpoint
    {
        public static void MapGetBasketEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/basket/items", async (
                GetBasketHandler handler
            ) =>
            {
                var result = await handler.HandleAsync();
                return Results.Ok(new ApiResponse<BasketResponse>(StatusCodes.Status200OK, "Success", result));
            })
            //.RequireAuthorization("RequireUserRole")
            .WithTags("Basket")

            .Produces<ApiResponse<BasketResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<BasketResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<BasketResponse>>(StatusCodes.Status401Unauthorized)
            .DisableAntiforgery();
        }
    }
}
