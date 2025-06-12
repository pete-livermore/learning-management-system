using Application.Common.Errors;

namespace Application.Common.Interfaces.Persistence;

public class PersistenceErrors
{
    private const string Context = "Persistence";
    private const string ConcurrencyCode = $"{Context}.Concurrency";
    private const string ConflictCode = $"{Context}.Conflict";
    private const string InvalidReferenceCode = $"{Context}.InvalidReference";
    private const string DataIntegrityCode = $"{Context}.DataIntegrity";
    private const string UnexpectedCode = $"{Context}.Unexpected";

    public static Error Conflict() =>
        new(
            ErrorType.Conflict,
            ConflictCode,
            "A record with conflicting unique values already exists. Please check your input."
        );

    public static Error InvalidReference() =>
        new(
            ErrorType.Validation,
            InvalidReferenceCode,
            "The provided data references a non-existent or invalid related record."
        );

    public static Error DataIntegrity() =>
        new(
            ErrorType.Validation,
            DataIntegrityCode,
            "The provided data violates a database integrity rule (e.g., incorrect length, invalid format, or missing required value)."
        );

    public static Error Concurrency(string? details) =>
        new(
            ErrorType.Concurrency,
            ConcurrencyCode,
            "The record you're trying to save has been modified by someone else. Please refresh and try again.",
            details
        );

    public static Error Unexpected(string? details)
    {
        var errorMessage =
            "A database error occurred during the update operation that could not be specifically classified. Please try again or contact support.";
        return new(ErrorType.Unexpected, UnexpectedCode, errorMessage, details);
    }
}
