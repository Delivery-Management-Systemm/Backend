using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FMS.ServiceLayer.Interface;

namespace FMS.ServiceLayer.Implementation
{
    public class CloudinaryService 
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["CloudinarySettings:CloudName"],
                config["CloudinarySettings:ApiKey"],
                config["CloudinarySettings:ApiSecret"]);
            _cloudinary = new Cloudinary(account);
        }
       
        public async Task<ImageUploadResult> UploadAsync(ImageUploadParams uploadParams)
        {
            if (uploadParams == null)
                throw new ArgumentNullException(nameof(uploadParams));

            // Gọi hàm upload của Cloudinary SDK
            var result = await _cloudinary.UploadAsync(uploadParams);

            // Kiểm tra kết quả
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"Upload failed: {result.Error?.Message}");
            }

            return result;
        }
    }
}

