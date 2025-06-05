using Application.Common.Dtos;
using Application.Common.Interfaces.Repositories;
using Application.UseCases.Security.Errors;
using Application.UseCases.Security.Interfaces;
using Application.UseCases.Uploads.Dtos;
using Application.UseCases.Uploads.Errors;
using Application.UseCases.Uploads.Helpers;
using Application.UseCases.Uploads.Interfaces;
using Application.Wrappers.Results;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Uploads.Commands;

public record CreateFileCommand : IRequest<Result<FileDto>>
{
    public required IFormFile FileContent { get; init; }
    public FileMetadataDto? FileMetadata { get; init; }
}

public class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, Result<FileDto>>
{
    private readonly IUploadsService _uploadsService;
    private readonly IFilesRepository _filesRepository;
    private readonly IUserAccessor _userAccessor;

    public CreateFileCommandHandler(
        IFilesRepository filesRepository,
        IUploadsService uploadsService,
        IUserAccessor userAccessor
    )
    {
        _filesRepository = filesRepository;
        _uploadsService = uploadsService;
        _userAccessor = userAccessor;
    }

    public async Task<Result<FileDto>> Handle(
        CreateFileCommand command,
        CancellationToken cancellationToken
    )
    {
        var file = command.FileContent;
        var fileMetadata = command.FileMetadata;
        bool isValidFile = FileValidator.IsValidFile(file);

        if (!isValidFile)
        {
            return Result<FileDto>.Failure(UploadErrors.Validation("Invalid file"));
        }

        string fileMimeType = file.ContentType;
        var uploadResult = await _uploadsService.Upload(file, fileMetadata);
        var providerMetadata = uploadResult.ProviderMetadata;
        string providerId = providerMetadata.Id;
        ;
        var authenticatedUser = _userAccessor.GetCurrentUser();

        if (authenticatedUser == null)
        {
            return Result<FileDto>.Failure(SecurityErrors.Unauthorized());
        }

        string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        UploadFile newFileRecord = new()
        {
            Url = uploadResult.Url,
            Mime = fileMimeType,
            Ext = fileExtension,
            ProviderId = providerId,
            OwnerId = authenticatedUser.Id,
        };

        await _filesRepository.Add(newFileRecord);

        FileDto fileDto = new()
        {
            Url = newFileRecord.Url,
            Mime = newFileRecord.Mime,
            Ext = newFileRecord.Ext,
            Owner = new EntityOwnerDto { Id = newFileRecord.OwnerId },
            ProviderId = newFileRecord.ProviderId,
            CreatedAt = newFileRecord.CreatedAt,
            UpdatedAt = newFileRecord.CreatedAt,
        };
        return Result<FileDto>.Success(fileDto);
    }
}
