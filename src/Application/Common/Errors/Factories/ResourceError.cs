namespace Application.Common.Errors.Factories;

public static class ResourceError
{
    public const string ConflictCode = "RESOURCE_CONFLICT";
    public const string NotFoundCode = "RESOURCE_NOT_FOUND";
    public const string ConcurrencyCode = "RESOURCE_CONCURRENCY";

    public static Error Conflict(string description) =>
        new(ErrorType.ResourcePersistence, ConflictCode, description);

    public static Error NotFound(string description) =>
        new(ErrorType.ResourcePersistence, NotFoundCode, description);

    public static Error Concurrency(string description) =>
        new(ErrorType.ResourcePersistence, ConcurrencyCode, description);
}
