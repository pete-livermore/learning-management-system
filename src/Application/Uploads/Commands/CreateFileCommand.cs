using Application.Common.Dtos;
using Application.Common.Errors.Factories;
using Application.Common.Interfaces.Repositories;
using Application.Common.Wrappers.Results;
using Application.Security.Interfaces;
using Application.Uploads.Dtos;
using Application.UseCases.Uploads.Interfaces;
using Domain.Uploads.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Uploads.Commands;

public record CreateFileCommand : IRequest<Result<FileDto>>
{
    public required IFormFile FileContent { get; init; }
    public FileMetadataDto? FileMetadata { get; init; }
}

public class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, Result<FileDto>>
{
    private readonly IFileValidator _fileValidator;
    private readonly IUploadsService _uploadsService;
    private readonly IFilesRepository _filesRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateFileCommandHandler(
        IFileValidator fileValidator,
        IFilesRepository filesRepository,
        IUploadsService uploadsService,
        ICurrentUserProvider currentUserProvider
    )
    {
        _fileValidator = fileValidator;
        _filesRepository = filesRepository;
        _uploadsService = uploadsService;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<FileDto>> Handle(
        CreateFileCommand command,
        CancellationToken cancellationToken
    )
    {
        var file = command.FileContent;
        var fileMetadata = command.FileMetadata;
        bool isValidFile = _fileValidator.IsValidFile(file);

        if (!isValidFile)
        {
            return Result<FileDto>.Failure(ValidationError.InvalidInput("Invalid file"));
        }

        string fileMimeType = file.ContentType;
        var uploadResult = await _uploadsService.Upload(file, fileMetadata);
        var providerMetadata = uploadResult.ProviderMetadata;
        string providerId = providerMetadata.Id;
        ;
        var currentUserDto = _currentUserProvider.GetCurrentUser();

        if (currentUserDto == null)
        {
            return Result<FileDto>.Failure(
                SecurityError.Unauthorized("The current user in not authenticated")
            );
        }

        string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        UploadFile newFileRecord = new()
        {
            Url = uploadResult.Url,
            Mime = fileMimeType,
            Ext = fileExtension,
            ProviderId = providerId,
            OwnerId = currentUserDto.Id,
        };

        await _filesRepository.Add(newFileRecord);

        FileDto fileDto = new()
        {
            Url = newFileRecord.Url,
            Mime = newFileRecord.Mime,
            Ext = newFileRecord.Ext,
            Owner = new EntityOwnerDto { Id = currentUserDto.Id.ToString() },
            ProviderId = newFileRecord.ProviderId,
            CreatedAt = newFileRecord.CreatedAt,
            UpdatedAt = newFileRecord.CreatedAt,
        };
        return Result<FileDto>.Success(fileDto);
    }
}
