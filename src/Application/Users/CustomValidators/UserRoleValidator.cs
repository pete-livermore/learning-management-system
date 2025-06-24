using Domain.Users.Enums;
using FluentValidation;

namespace Application.Users.CustomValidators;

public static class UserRoleValidator
{
    private static bool BeAValidUserRole(string role)
    {
        return Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsed)
            && Enum.IsDefined(typeof(UserRole), parsed);
    }

    public static IRuleBuilderOptions<T, string> MustBeValidUserRole<T>(
        this IRuleBuilder<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .NotEmpty()
            .Must(BeAValidUserRole)
            .WithMessage("'{PropertyName}' must be a valid user role.");
    }
}
