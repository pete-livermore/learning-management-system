using Application.Uploads.Dtos;
using Application.UseCases.Uploads.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Infrastructure.Uploads.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Uploads.Services
{
    public class UploadsService : IUploadsService
    {
        private readonly Cloudinary _cloudinary;

        public UploadsService(IOptions<CloudinaryOptions> cloudinaryConfig)
        {
            _cloudinary = new Cloudinary(cloudinaryConfig.Value.Url);
        }

        public async Task<UploadedFileDto> Upload(
            IFormFile file,
            FileMetadataDto? fileMetadata = null
        )
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true,
            };
            var cloudinaryUploadResult = await _cloudinary.UploadAsync(uploadParams);

            return new UploadedFileDto()
            {
                Url = cloudinaryUploadResult.Url.ToString(),
                ResourceType = cloudinaryUploadResult.ResourceType,
                ProviderMetadata = new UploadProviderMetadata()
                {
                    Id = cloudinaryUploadResult.PublicId,
                },
            };
        }
    }
}
