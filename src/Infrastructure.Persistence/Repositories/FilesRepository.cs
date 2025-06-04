using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence.Repositories
{
    public class FilesRepository : IFilesRepository
    {
        private readonly LearningManagementSystemDbContext _context;

        public FilesRepository(LearningManagementSystemDbContext context)
        {
            _context = context;
        }

        public async Task<UploadFile> Add(UploadFile newUploadedFile)
        {
            _context.Files.Add(newUploadedFile);
            await _context.SaveChangesAsync();

            return newUploadedFile;
        }
    }
}
