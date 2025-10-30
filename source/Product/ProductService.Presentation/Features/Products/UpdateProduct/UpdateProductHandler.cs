using AutoMapper;
using MassTransit;
using MongoDB.Driver;
using Newtonsoft.Json;
using ProductService.Presentation.Data;
using ProductService.Presentation.Entities;
using ProductService.Presentation.Entities.Events;
using ProductService.Presentation.Features.Products.CreateProduct;
using ProductService.Presentation.Services;
using SharedLibrarySolution.Exceptions;

namespace ProductService.Presentation.Features.Products.UpdateProduct
{
    public class UpdateProductHandler
    {
        private readonly MongoDbContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public UpdateProductHandler(MongoDbContext context,
                                    CloudinaryService cloudinaryService,
                                    IMapper mapper,
                                    IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ProductsResponse?> HandleAsync(string id, UpdateProductRequest request)
        {
            var product = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null)
                throw new AppException("Product not found");

            // Lưu giá trị cũ
            var oldPrice = product.Price;
            var oldName = product.Name;
            var oldImageUrl = product.ImageUrls?.FirstOrDefault() ?? "";

            // Parse variants
            List<ProductVariantRequestDto>? variantDtos = null;
            if (!string.IsNullOrEmpty(request.VariantsJson))
            {
                variantDtos = JsonConvert.DeserializeObject<List<ProductVariantRequestDto>>(request.VariantsJson);
            }

            // Xử lý ảnh
            List<string> imageUrls = product.ImageUrls?.ToList() ?? new List<string>();

            if (!string.IsNullOrEmpty(request.OldImageUrls))
            {
                try
                {
                    var oldUrls = JsonConvert.DeserializeObject<List<string>>(request.OldImageUrls)
                        ?.Where(url => !string.IsNullOrWhiteSpace(url))
                        .ToList();
                    if (oldUrls != null)
                        imageUrls = oldUrls;
                }
                catch
                {
                    throw new AppException("Invalid OldImageUrls JSON format");
                }
            }

            // Upload ảnh mới
            if (request.Images != null && request.Images.Any())
            {
                foreach (var img in request.Images)
                {
                    var imgUrl = await _cloudinaryService.UploadFileAsync(img);
                    imageUrls.Add(imgUrl);
                }
            }

            // Cập nhật dữ liệu
            if (!string.IsNullOrWhiteSpace(request.Name))
                product.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Description))
                product.Description = request.Description;

            if (request.Price != 0)
                product.Price = request.Price;

            if (!string.IsNullOrWhiteSpace(request.CategoryId))
                product.CategoryId = request.CategoryId;

            if (variantDtos != null)
            {
                product.Variants = variantDtos.Select(v => new ProductVariant
                {
                    Sku = v.Sku,
                    Price = v.Price,
                }).ToList();
            }

            product.ImageUrls = imageUrls;
            product.UpdatedAt = DateTime.UtcNow;

            // Lưu vào MongoDB
            await _context.Products.ReplaceOneAsync(p => p.Id == id, product);

            // Kiểm tra thay đổi

            var hasImportantChanges =
                oldPrice != product.Price ||
                oldName != product.Name;
               

            if (hasImportantChanges)
            {
                var @event = new ProductUpdatedEvent
                {
                    ProductId = product.Id!,
                    ProductName = product.Name,
                    Price = product.Price,
                    UpdatedAt = DateTime.UtcNow
                };

                // PUBLISH với ROUTING KEY
                Console.WriteLine("📢 Publishing to product_exchange with routing key: product.updated");
                await _publishEndpoint.Publish(@event, ctx =>
                {
                    ctx.SetRoutingKey("product.updated");
                });

                Console.WriteLine($"==>Published: ProductId={@event.ProductId}, Name={@event.ProductName}, Price=${@event.Price}");
            }

            return _mapper.Map<ProductsResponse>(product);
        }
    }
}