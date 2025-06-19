namespace Application.Common.Errors.Factories;

public static class ValidationError
{
    public static Error InvalidInput(string description) =>
        new(ErrorType.Validation, "VALIDATION_INVALID_INPUT", description);

    public static Error FieldRequired(string fieldName) =>
        new(
            ErrorType.Validation,
            "VALIDATION_FIELD_REQUIRED",
            $"The '{fieldName}' field is required."
        );
}
