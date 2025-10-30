using AutoMapper;
using MongoDB.Driver;
using ProductService.Presentation.Data;
using ProductService.Presentation.Entities;
using ProductService.Presentation.Services;
using Newtonsoft.Json;
using SharedLibrarySolution.Exceptions;


namespace ProductService.Presentation.Features.Products.CreateProduct
{
    public class CreateProductHandler
    {
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinaryService;

        public CreateProductHandler(MongoDbContext context, IMapper mapper, CloudinaryService cloudinaryService)
        {
            _context = context;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ProductsResponse?> HandleAsync(CreateProductRequest request)
        {
            // VALIDATE VariantsJson -> chuyển chuỗi text lúc gửi dạng frmdata về json để lưu list object
            var variantDtos = JsonConvert.DeserializeObject<List<ProductVariantRequestDto>>(request.VariantsJson);
            if (variantDtos == null)
                throw new AppException("Invalid Variants JSON");

            //upload anh sp
            var imageUrls = new List<string>();
            if (request.Images != null && request.Images.Any())
            {
                foreach (var img in request.Images)
                {
                    var url = await _cloudinaryService.UploadFileAsync(img);
                    imageUrls.Add(url);
                }
            }


            // TẠO PRODUCT
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Variants = variantDtos.Select(v => new ProductVariant
                {
                    Sku = v.Sku,
                    Price = v.Price,
                }).ToList(),
                ImageUrls = imageUrls
            };

            // luu xuong csdl
            await _context.Products.InsertOneAsync(product);

            // MAP dto trả response
            return _mapper.Map<ProductsResponse>(product);
        }

    }
}