using Application.Dtos.User;
using Application.Errors;
using Application.Interfaces.Users;
using Application.Wrappers.Results;
using MediatR;

namespace Application.UseCases.Users.Commands;

public record class ReplaceUserCommand : IRequest<Result<UserDto>>
{
    public required int UserId { get; init; }
    public required ReplaceUserDto ReplaceCommand { get; init; }
}

public class ReplaceUserCommandHandler : IRequestHandler<ReplaceUserCommand, Result<UserDto>>
{
    private readonly IUsersRepository _usersRepository;

    public ReplaceUserCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Result<UserDto>> Handle(
        ReplaceUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = request.UserId;
        var replaceUserDto = request.ReplaceCommand;
        var existingUser = await _usersRepository.FindById(userId);

        if (existingUser is null)
        {
            return Result<UserDto>.Failure(UserErrors.NotFound(userId));
        }

        existingUser.Email = replaceUserDto.Email;
        existingUser.FirstName = replaceUserDto.FirstName;
        existingUser.LastName = replaceUserDto.LastName;
        existingUser.Password = replaceUserDto.Password;
        existingUser.Role = existingUser.Role;

        var updatedUserRecord = await _usersRepository.Update(existingUser);

        var updatedUserDto = new UserDto()
        {
            Id = updatedUserRecord.Id,
            FirstName = updatedUserRecord.FirstName,
            LastName = updatedUserRecord.LastName,
            Email = updatedUserRecord.Email,
            Role = updatedUserRecord.Role.ToString(),
        };

        return Result<UserDto>.Success(updatedUserDto);
    }
}
