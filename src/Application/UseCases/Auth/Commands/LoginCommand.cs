using Application.Dtos.Auth;
using Application.Errors;
using Application.Interfaces.Auth;
using Application.Interfaces.Users;
using Application.Wrappers.Results;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Auth.Commands;

public record class LoginCommand : IRequest<Result<string>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly ITokenService _tokenService;
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public LoginCommandHandler(
        ITokenService tokenService,
        IUsersRepository usersRepository,
        IPasswordHasher<User> passwordHasher
    )
    {
        _tokenService = tokenService;
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<string>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        string loginEmail = request.Email;
        string loginPassword = request.Password;
        var user = await _usersRepository.FindByEmail(loginEmail);

        if (user is null)
        {
            return Result<string>.Failure(AuthErrors.InvalidUser(loginEmail));
        }
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.Password,
            request.Password
        );

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return Result<string>.Failure(AuthErrors.InvalidPassword(loginEmail, loginPassword));
        }

        string token = _tokenService.Generate(
            new TokenDataDto()
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
            }
        );

        return Result<string>.Success(token);
    }
}
