using Application.Common.Errors;

namespace Application.Common.Wrappers.Results;

public class Result
{
    public bool IsSuccess { get; }
    public IReadOnlyList<Error> Errors { get; }

    public bool IsFailure => !IsSuccess;

    public Result(bool isSuccess, params Error[] errors)
    {
        var hasErrors = errors.Any(err => err != Error.None);
        if (isSuccess && hasErrors || !isSuccess && !hasErrors)
        {
            throw new ArgumentException("Invalid errors", nameof(errors));
        }

        IsSuccess = isSuccess;

        Errors = errors;
    }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(params Error[] errors) => new(false, errors);
}

public class Result<T> : Result
{
    public T Value { get; }

    public Result(bool isSuccess, T value, params Error[] errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T data) => new(true, data, Error.None);

    public static new Result<T> Failure(params Error[] errors) => new(false, default!, errors);
}
