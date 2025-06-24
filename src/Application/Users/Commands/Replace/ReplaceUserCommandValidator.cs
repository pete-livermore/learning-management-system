using Application.Users.CustomValidators;
using FluentValidation;

namespace Application.Users.Commands.Replace;

public class ReplaceUserCommandValidator : AbstractValidator<ReplaceUserCommand>
{
    public ReplaceUserCommandValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(50);
        RuleFor(c => c.Email).EmailAddress().NotEmpty();
        RuleFor(c => c.Password).MustBeValidPassword();
        RuleFor(c => c.Role).MustBeValidUserRole();
    }
}
