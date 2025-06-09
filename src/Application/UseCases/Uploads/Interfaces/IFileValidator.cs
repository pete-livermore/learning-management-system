using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Uploads.Interfaces;

public interface IFileValidator
{
    public bool IsValidFile(IFormFile file);
}
