using Domain.Users.Enums;
using FluentValidation;

namespace Application.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(50);
        RuleFor(c => c.Email).EmailAddress().NotEmpty();
        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(50);
        RuleFor(c => c.Role).Must(BeAValidUserRole).WithMessage("Role must be a valid user role.");
    }

    private bool BeAValidUserRole(string role)
    {
        return Enum.TryParse<UserRole>(role, out var parsed)
            && Enum.IsDefined(typeof(UserRole), parsed);
    }
}
