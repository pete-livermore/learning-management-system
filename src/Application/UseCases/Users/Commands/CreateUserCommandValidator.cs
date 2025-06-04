using Domain.Enums;
using FluentValidation;

namespace Application.UseCases.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.CreateCommand)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(c => c.CreateCommand.FirstName).NotEmpty().MaximumLength(50);
                RuleFor(c => c.CreateCommand.LastName).NotEmpty().MaximumLength(50);
                RuleFor(c => c.CreateCommand.Email).EmailAddress().NotEmpty();
                RuleFor(c => c.CreateCommand.Password)
                    .NotEmpty()
                    .MinimumLength(8)
                    .WithMessage("Password must be at least 8 characters long.")
                    .MaximumLength(50);

                RuleFor(c => c.CreateCommand.Role)
                    .Must(BeAValidUserRole)
                    .WithMessage("Role must be a valid user role.");
            });
    }

    private bool BeAValidUserRole(string role)
    {
        return Enum.TryParse<UserRole>(role, out var parsed)
            && Enum.IsDefined(typeof(UserRole), parsed);
    }
}
