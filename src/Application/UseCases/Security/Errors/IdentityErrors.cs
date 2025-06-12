using Application.Common.Errors;

namespace Application.UseCases.Security.Errors;

public class IdentityErrors
{
    private const string ErrorContext = "Identity";
    private const string NotFoundCode = $"{ErrorContext}.NotFound";
    private const string ValidationCode = $"{ErrorContext}.Validation";
    private const string ConflictCode = $"{ErrorContext}.Conflict";
    private const string ForbiddenCode = $"{ErrorContext}.Forbidden";
    private const string UnexpectedCode = $"{ErrorContext}.Unexpected";

    public static Error NotFound(Guid userId) =>
        new(ErrorType.NotFound, NotFoundCode, $"Application user with ID {userId} not found");

    public static Error Validation(string userEmail, string? details = "") =>
        new(
            ErrorType.Validation,
            ValidationCode,
            $"The user with email {userEmail} has invalid data",
            details
        );

    public static Error Conflict(string userEmail) =>
        new(
            ErrorType.Conflict,
            ConflictCode,
            $"Application user with ID {userEmail} already exists"
        );

    public static Error Forbidden(string? message = "") =>
        new(
            ErrorType.Forbidden,
            ForbiddenCode,
            message ?? "The user is forbidden to access this resource"
        );

    public static Error Unexpected(string? details = "") =>
        new(ErrorType.Unexpected, UnexpectedCode, "An unexpected identity error occurred", details);
}
