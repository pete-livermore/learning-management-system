namespace Application.Common.Errors.Factories;

public static class SecurityError
{
    public const string InvalidCredentialsCode = "AUTH_INVALID_CREDENTIALS";
    public const string UnauthorizedCode = "AUTH_UNAUTHORIZED";
    public const string ForbiddenCode = "AUTH_FORBIDDEN";

    public static Error InvalidCredentials(string description, string? details) =>
        new(ErrorType.Security, InvalidCredentialsCode, description, details);

    public static Error MissingToken(string description) =>
        new(ErrorType.Security, "AUTH_MISSING_TOKEN", description);

    public static Error InvalidToken(string description) =>
        new(ErrorType.Security, "AUTH_INVALID_TOKEN", description);

    public static Error Unauthorized(string description) =>
        new(ErrorType.Security, UnauthorizedCode, description);

    public static Error Forbidden(string description) =>
        new(ErrorType.Security, ForbiddenCode, description);

    public static Error AccountLocked(string description) =>
        new(ErrorType.Security, "AUTH_ACCOUNT_LOCKED", description);

    public static Error AccountUnconfirmed(string description) =>
        new(ErrorType.Security, "AUTH_ACCOUNT_NOT_CONFIRMED", description);
}
