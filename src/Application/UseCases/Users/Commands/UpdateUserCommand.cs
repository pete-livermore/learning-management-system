using Application.Dtos.User;
using Application.Errors;
using Application.Interfaces.Users;
using Application.Utilities.Dto;
using Application.Wrappers.Results;
using MediatR;

namespace Application.UseCases.Users.Commands;

public record class UpdateUserCommand : IRequest<Result<UserDto>>
{
    public required int UserId { get; init; }
    public required UpdateUserDto UpdateCommand { get; init; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUsersRepository _usersRepository;

    public UpdateUserCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
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
        var updatedUserModel = DtoUtilities.Map(
            existingUser,
            updateUserDto,
            new DtoMapOptions() { IgnoreNull = true }
        );
        var updatedUserRecord = await _usersRepository.Update(updatedUserModel);

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
