using Application.Errors;

namespace Application.Results;

public class Result
{
    public bool IsSuccess { get; }
    public Error Error { get; }

    public bool IsFailure => !IsSuccess;

    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;

        Error = error;
    }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);
}

public class Result<T> : Result
{
    public T Value { get; }

    public Result(bool isSuccess, T value, Error error)
        : base(isSuccess, error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        Value = value;
    }

    public static Result<T> Success(T data) => new(true, data, Error.None);

    public static new Result<T> Failure(Error error) => new(false, default!, error);
}
