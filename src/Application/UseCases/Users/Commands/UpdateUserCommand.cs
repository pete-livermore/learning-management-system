using Application.Dtos.User;
using Application.Errors;
using Application.Interfaces.Users;
using Application.Utilities.Dto;
using Application.Wrappers.Results;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Users.Commands;

public record class UpdateUserCommand : IRequest<Result<UserDto>>
{
    public required int UserId { get; init; }
    public required UpdateUserDto UpdateCommand { get; init; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UpdateUserCommandHandler(
        IUsersRepository usersRepository,
        IPasswordHasher<User> passwordHasher
    )
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = request.UserId;
        var existingUser = await _usersRepository.FindById(userId);

        if (existingUser is null)
        {
            return Result<UserDto>.Failure(UserErrors.NotFound(userId));
        }

        var updateUserDto = request.UpdateCommand;

        if (updateUserDto.Email is not null)
        {
            existingUser.Email = updateUserDto.Email;
        }

        if (updateUserDto.FirstName is not null)
        {
            existingUser.FirstName = updateUserDto.FirstName;
        }

        if (updateUserDto.LastName is not null)
        {
            existingUser.LastName = updateUserDto.LastName;
        }

        if (updateUserDto.Password is not null)
        {
            string hashedPassword = _passwordHasher.HashPassword(
                existingUser,
                updateUserDto.Password
            );
            existingUser.Password = hashedPassword;
        }

        var updatedUserRecord = await _usersRepository.Update(existingUser);

        var updatedUserDto = new UserDto()
        {
            Id = updatedUserRecord.Id,
            Email = updatedUserRecord.Email,
            FirstName = updatedUserRecord.FirstName,
            LastName = updatedUserRecord.LastName,
            Role = updatedUserRecord.Role.ToString(),
        };

        return Result<UserDto>.Success(updatedUserDto);
    }
}
