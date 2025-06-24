using FluentValidation;

namespace Application.Security.Commands;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.Email).EmailAddress().NotEmpty();
        RuleFor(c => c.Password).NotEmpty();
    }
}
