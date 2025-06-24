using Domain.Users.Enums;
using FluentValidation;

namespace Application.Users.CustomValidators;

public static class PasswordValidator
{
    public static IRuleBuilderOptions<T, string> MustBeValidPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(50);
    }
}
