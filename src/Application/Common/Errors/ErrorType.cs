namespace Application.Common.Errors;

public enum ErrorType
{
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    Unexpected,
    Validation,
    Concurrency,
    Network,
    None,
}
