using FluentValidation;
using Newtonsoft.Json;
using ProductService.Presentation.Features.Products.CreateProduct;

namespace ProductService.Presentation.Features.Products.UpdateProduct
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name too long");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be >= 0");

            When(x => !string.IsNullOrEmpty(x.CategoryId), () =>
                RuleFor(x => x.CategoryId).NotEmpty().WithMessage("CategoryId cannot be empty")
            );

            RuleFor(x => x.VariantsJson)
                 .Must(BeValidJson)
                 .When(x => !string.IsNullOrEmpty(x.VariantsJson))
                 .WithMessage("Variants must be valid JSON array");
        }

        private static bool BeValidJson(string json)
        {
            try
            {
                JsonConvert.DeserializeObject<List<ProductVariantRequestDto>>(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

