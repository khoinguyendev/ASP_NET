using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace ASP_NET.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly long _maxFileSizeInBytes = 3 * 1024 * 1024; // Ví dụ: Giới hạn kích thước 5MB
        private readonly List<string> _allowedFileTypes = new List<string> { "image/jpeg", "image/png", "image/gif","image/webp" };

        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> files)
        {
            // Danh sách chứa URL của các ảnh đã upload
            var uploadedImageUrls = new List<string>();

       
            // Kiểm tra kích thước và loại file của tất cả các file
            foreach (var file in files)
            {
                // Kiểm tra kích thước file
                if (file.Length > _maxFileSizeInBytes)
                {
                    throw new Exception($"File size of {file.FileName} exceeds the maximum limit of {_maxFileSizeInBytes / 1024 / 1024}MB.");
                }

                // Kiểm tra loại file
                if (!_allowedFileTypes.Contains(file.ContentType))
                {
                    throw new Exception($"Invalid file type for {file.FileName}. Only JPG, PNG, and GIF are allowed.");
                }
            }

            // Nếu tất cả file đều hợp lệ, tiến hành upload lên Cloudinary
            foreach (var file in files)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    PublicId = Guid.NewGuid().ToString() // Optional: set publicId here if needed
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception("Error uploading image: " + uploadResult.Error.Message);
                }

                // Thêm URL ảnh đã upload vào danh sách
                uploadedImageUrls.Add(uploadResult.SecureUrl.AbsoluteUri);
            }

            return uploadedImageUrls;
        }
    }
}