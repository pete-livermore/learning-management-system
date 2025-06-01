using Application.Dtos.User;
using Application.Errors;
using Application.Interfaces.Users;
using Application.Wrappers.Results;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Users.Commands;

public record class CreateUserCommand : IRequest<Result<UserDto>>
{
    public required CreateUserDto CreateCommand { get; init; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public CreateUserCommandHandler(
        IUsersRepository usersRepository,
        IPasswordHasher<User> passwordHasher
    )
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var createUserDto = request.CreateCommand;
        var existingUser = await _usersRepository.FindByEmail(createUserDto.Email);

        if (existingUser != null)
        {
            return Result<UserDto>.Failure(UserErrors.Conflict(createUserDto.Email));
        }

        var newUser = new User()
        {
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Role = createUserDto.Role,
            Password = "",
        };
        newUser.Password = _passwordHasher.HashPassword(newUser, createUserDto.Password);
        var createdUserRecord = await _usersRepository.Add(newUser);

        var newUserDto = new UserDto()
        {
            Email = createdUserRecord.Email,
            FirstName = createdUserRecord.FirstName,
            LastName = createdUserRecord.LastName,
            Role = createdUserRecord.Role.ToString(),
        };

        return Result<UserDto>.Success(newUserDto);
    }
}
