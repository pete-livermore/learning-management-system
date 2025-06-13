using System;

namespace Application.Common.Errors.Factories;

public static class UnexpectedError
{
    public static Error Unknown(string? description, string? details = "") =>
        new(
            ErrorType.Unexpected,
            "UNKOWN_ERROR",
            description ?? "An unknown error occurred",
            details
        );
}
