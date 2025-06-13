using Application.Common.Interfaces.Security;
using Application.Wrappers.Results;
using MediatR;

namespace Application.UseCases.Security.Commands;

public record class LoginCommand : IRequest<Result<string>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<string>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        string suppliedEmail = request.Email;
        string suppliedPassword = request.Password;
        return await _identityService.AuthenticateUserAsync(suppliedEmail, suppliedPassword);
    }
}
