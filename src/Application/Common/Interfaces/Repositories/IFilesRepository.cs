using Domain.Entities;

namespace Application.Common.Interfaces.Repositories;

public interface IFilesRepository
{
    Task<UploadFile> Add(UploadFile file);
}
