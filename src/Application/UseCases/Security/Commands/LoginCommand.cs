using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Token;
using Application.UseCases.Security.Dtos;
using Application.UseCases.Security.Errors;
using Application.Wrappers.Results;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Security.Commands;

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
        string suppliedEmail = request.Email;
        string suppliedPassword = request.Password;
        var user = await _usersRepository.FindByEmail(suppliedEmail);

        if (user is null)
        {
            return Result<string>.Failure(SecurityErrors.Unauthorized());
        }
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.Password,
            suppliedPassword
        );

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return Result<string>.Failure(SecurityErrors.Unauthorized());
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
