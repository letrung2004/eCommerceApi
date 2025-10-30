using Microsoft.AspNetCore.Mvc;
using SharedLibrarySolution.Responses;

namespace BasketService.Presentation.Features.Baskets.AddItemToBasket
{
    public static class AddItemEndpoint
    {
        // test 
        //{
        //  "productId": "P001",
        //  "productName": "Laptop Dell XPS 15",
        //  "quantity": 2,
        //  "price": 1500.00,
        //  "discountPrice": 1300.00
        //}

        public static void MapAddItemEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapPost("/basket/items", async (
                [FromBody] AddItemRequest request,
                AddItemHandler handler
            ) =>
            {
                var result = await handler.HandleAsync(request);
                return Results.Ok(new ApiResponse<BasketResponse>(StatusCodes.Status200OK, "Success", result));
            })
            .RequireAuthorization()
            .WithTags("Basket")
            .WithName("AddItemToBasket")

            .Produces<ApiResponse<BasketResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<BasketResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<BasketResponse>>(StatusCodes.Status401Unauthorized)
            .DisableAntiforgery();
        }
    }
}

