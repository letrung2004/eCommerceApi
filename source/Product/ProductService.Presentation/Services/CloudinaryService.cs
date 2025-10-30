using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace ProductService.Presentation.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var account = new Account(
                "dehrl068l",
                "489984614857154",
                "G6YCpw_DQ0IMnSRU1P7SDoMrUw4"
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "products"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }
    }
}
