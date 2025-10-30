using FluentValidation;
using Newtonsoft.Json;

namespace ProductService.Presentation.Features.Products.CreateProduct
{
    public class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name too long");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be >= 0");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required");

            // VALIDATE VariantsJson
            RuleFor(x => x.VariantsJson)
                .NotEmpty().WithMessage("Variants is required")
                .Must(BeValidJson).WithMessage("Variants must be valid JSON array");

            RuleForEach(x => x.Images)
                .Must(file => file != null && file.Length > 0)
                .WithMessage("Invalid image file")
                .When(x => x.Images != null);
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