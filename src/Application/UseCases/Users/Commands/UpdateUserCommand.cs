using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Security;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Errors;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsersRepository _usersRepository;
    private readonly IIdentityService _identityService;

    public UpdateUserCommandHandler(
        IUsersRepository usersRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork
    )
    {
        _usersRepository = usersRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = request.UserId;
        var existingUser = await _usersRepository.FindByIdAsync(userId);

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

        await _identityService.UpdateUser(
            existingUser.ApplicationUserId,
            new UpdateApplicationUserDto() { Email = updateUserDto.Email }
        );

        _usersRepository.Update(existingUser);
        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
        {
            return Result<UserDto>.Failure([.. saveResult.Errors]);
        }

        var updatedUserDto = new UserDto()
        {
            Id = existingUser.Id,
            Email = existingUser.Email,
            FirstName = existingUser.FirstName,
            LastName = existingUser.LastName,
            Role = existingUser.Role.ToString(),
        };

        return Result<UserDto>.Success(updatedUserDto);
    }
}
