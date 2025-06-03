using Application.Common.Errors;

namespace Application.UseCases.Security.Errors;

public static class SecurityErrors
{
    private const string ErrorContext = "Security";
    public const string UnauthorizedCode = $"{ErrorContext}.Unauthorized";
    public const string ForbiddenCode = $"{ErrorContext}.Forbidden";

    public static Error Unauthorized() =>
        new(
            ErrorType.Unauthorized,
            UnauthorizedCode,
            "User must be logged in to access this resource"
        );

    public static Error Forbidden() =>
        new(
            ErrorType.Forbidden,
            ForbiddenCode,
            $"The current user does not have permission to perform this action"
        );
}
