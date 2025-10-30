using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrarySolution.Exceptions;
using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Products.UpdateProduct
{
    public static class UpdateProductEndpoint
    {
        public static void MapUpdateProductEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapPut("/products/{id}", async (
                string id, // do MapPut từ bind data vào id(giống tên) nên k cần FromQuery
                [FromForm] UpdateProductRequest request,
                UpdateProductHandler handler,
                IValidator<UpdateProductRequest> validator
            ) =>
            {
                try
                {
                    var validation = await validator.ValidateAsync(request);
                    if (!validation.IsValid)
                    {
                        var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                        return Results.BadRequest(new ApiResponse<ProductsResponse>(400, errors));
                    }

                    var result = await handler.HandleAsync(id, request);
                    if (result == null)
                        return Results.NotFound(new ApiResponse<ProductsResponse>(404, "Product not found"));

                    return Results.Ok(new ApiResponse<ProductsResponse>(200, "Product updated successfully", result));
                }
                catch (JsonException ex)
                {
                    throw new AppException(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    throw new AppException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new AppException(ex.Message);
                }
            })
            .RequireAuthorization("RequireAdminOrSeller") // cần đăng nhập và có permission thì mới dùng api này đc
            .WithTags("Products")
            .WithName("UpdateProduct")
            .Accepts<UpdateProductRequest>("multipart/form-data")
            .Produces<ApiResponse<ProductsResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<ProductsResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<ProductsResponse>>(StatusCodes.Status404NotFound)
            .DisableAntiforgery(); // để có thể test postman
        }
    }
}
