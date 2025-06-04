using Application.Common.Interfaces.Repositories;
using Application.UseCases.Uploads.Dtos;
using Application.UseCases.Uploads.Errors;
using Application.UseCases.Uploads.Interfaces;
using Application.Wrappers.Results;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Uploads.Commands;

public record CreateFileCommand : IRequest<Result<FileDto>>
{
    public required CreateFileDto CreateCommand { get; init; }
    public required IFormFile FileContent { get; init; }
    public FileMetadataDto? FileMetadata { get; init; }
}

public class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, Result<FileDto>>
{
    private readonly IUploadsService _uploadsService;
    private readonly IFilesRepository _filesRepository;

    public CreateFileCommandHandler(
        IFilesRepository filesRepository,
        IUploadsService uploadsService
    )
    {
        _filesRepository = filesRepository;
        _uploadsService = uploadsService;
    }

    public async Task<Result<FileDto>> Handle(
        CreateFileCommand command,
        CancellationToken cancellationToken
    )
    {
        var createFileDto = command.CreateCommand;

        var uploadResult = await _uploadsService.Upload(command.FileContent, command.FileMetadata);
        var providerMetadata = uploadResult.ProviderMetadata;
        string providerId = providerMetadata.Id;

        if (Enum.TryParse<UploadResourceType>(createFileDto.ResourceType, out var resourceType))
        {
            return Result<FileDto>.Failure(UploadErrors.Validation("Invalid resource type"));
        }
        ;

        UploadFile newFileRecord = new()
        {
            Url = uploadResult.Url,
            Mime = createFileDto.Mime,
            ResourceType = resourceType,
            Ext = createFileDto.Ext,
            ProviderId = providerId,
            OwnerId = createFileDto.OwnerId,
        };

        await _filesRepository.Add(newFileRecord);

        FileDto fileDto = new()
        {
            Url = newFileRecord.Url,
            Mime = newFileRecord.Mime,
            Ext = newFileRecord.Ext,
            Owner = new FileOwnerDto { Id = newFileRecord.OwnerId },
            ProviderId = newFileRecord.ProviderId,
            ResourceType = newFileRecord.ResourceType.ToString(),
            CreatedAt = newFileRecord.CreatedAt,
            UpdatedAt = newFileRecord.CreatedAt,
        };
        return Result<FileDto>.Success(fileDto);
    }
}
