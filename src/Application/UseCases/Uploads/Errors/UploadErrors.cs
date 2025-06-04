using Application.Common.Errors;

namespace Application.UseCases.Uploads.Errors;

public static class UploadErrors
{
    public static Error Validation(string message) =>
        new(ErrorType.Validation, "Uploads.Validation", message);
}
