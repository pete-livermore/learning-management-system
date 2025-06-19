using Application.Uploads.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Uploads.Interfaces;

public interface IUploadsService
{
    public Task<UploadedFileDto> Upload(IFormFile file, FileMetadataDto? uploadMetadata);
}
