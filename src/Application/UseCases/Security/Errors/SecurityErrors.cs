using Application.Common.Errors;

namespace Application.UseCases.Security.Errors;

public static class SecurityErrors
{
    private const string ErrorContext = "Security";
    public const string InvalidUserCode = $"{ErrorContext}.InvalidUser";
    public const string InvalidPasswordCode = $"{ErrorContext}.InvalidPassword";
    public const string UnauthorizedCode = $"{ErrorContext}.Unauthorized";
    public const string ForbiddenCode = $"{ErrorContext}.Forbidden";

    public static Error InvalidUser(string email) =>
        new(InvalidUserCode, $"The user with email '{email}' is not an authenticated user");

    public static Error InvalidPassword(string email) =>
        new(InvalidPasswordCode, $"The password for user {email} is not correct");

    public static Error Unauthorized() =>
        new(UnauthorizedCode, "User must be logged in to access this resource");

    public static Error Forbidden() =>
        new(ForbiddenCode, $"The current user does not have permission to perform this action");
}
