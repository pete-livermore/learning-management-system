using Application.Common.Configuration;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Application.UseCases.Uploads.Commands;

public class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
{
    public CreateFileCommandValidator()
    {
        RuleFor(c => c.FileContent)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(c => c.FileContent.ContentType)
                    .NotEmpty()
                    .WithMessage("File must have a content type.");
                RuleFor(c => c.FileContent.FileName)
                    .NotEmpty()
                    .WithMessage("File must be have a file name");
            });
    }
}
