using Application.Common.Errors;
using Application.Common.Wrappers.Results;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviours;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest>? _validator = validator;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (_validator is null)
        {
            return await next();
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }

        var validationErrors = validationResult
            .Errors.ConvertAll(error => new Error(
                Type: ErrorType.Validation,
                Code: error.PropertyName,
                Description: error.ErrorMessage
            ))
            .ToArray();

        // Having to use reflection to access T in Result<T> that is the response type
        var resultType = typeof(TResponse);
        var genericType = resultType.GetGenericArguments().FirstOrDefault();

        if (genericType == null)
            throw new InvalidOperationException(
                "ValidationBehavior expects TResponse to be Result<T>"
            );

        var failureMethod = typeof(Result<>)
            .MakeGenericType(genericType)
            .GetMethod("Failure", [typeof(Error[])]);

        var failureResult = failureMethod?.Invoke(null, [validationErrors]);

        return (TResponse)failureResult!;
    }
}
