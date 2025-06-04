using Application.Common.Configuration;
using Application.Common.Constants;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.UseCases.Uploads.Commands;

public class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
{
    private readonly UploadOptions _uploadOptions;

    public CreateFileCommandValidator(IOptions<UploadOptions> uploadOptions)
    {
        _uploadOptions = uploadOptions.Value;

        RuleFor(c => c.CreateCommand)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(c => c.FileContent)
                    .NotNull()
                    .WithMessage("A file must be provided.")
                    .Must(BeValidFileSize);

                RuleFor(c => c.CreateCommand.Url).NotEmpty();
                RuleFor(c => c.CreateCommand.Mime).Must(BeValidMimeType);
                RuleFor(c => c.CreateCommand.Ext).Must(BeValidExtension);
                RuleFor(c => c.CreateCommand.ResourceType).Must(BeValidResourceType);
            });
    }

    private bool BeValidFileSize(IFormFile file)
    {
        return file.Length > 0 && file.Length < _uploadOptions.MaxFileSizeBytes;
    }

    private bool BeValidMimeType(string value)
    {
        return AllowedFileFormats.GetMimeType(value) != null;
    }

    private bool BeValidExtension(string value)
    {
        string extension = value.TrimStart('.');
        return AllowedFileFormats.IsExtensionAllowed(extension);
    }

    private bool BeValidResourceType(string value)
    {
        return Enum.TryParse<UploadResourceType>(value, out var _);
    }
}
