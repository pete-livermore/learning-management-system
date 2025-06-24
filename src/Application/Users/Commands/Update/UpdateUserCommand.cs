using Application.Common.Interfaces.Repositories;
using Application.Common.Wrappers.Results;
using Application.Security.Dtos;
using Application.Security.Interfaces;
using Application.Users.Dtos;
using Application.Users.Errors;
using MediatR;

namespace Application.Users.Commands;

public record class UpdateUserCommand : IRequest<Result<UserDto>>
{
    public required int UserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
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

        if (request.Email is not null)
        {
            existingUser.Email = request.Email;
        }

        if (request.FirstName is not null)
        {
            existingUser.FirstName = request.FirstName;
        }

        if (request.LastName is not null)
        {
            existingUser.LastName = request.LastName;
        }

        await _identityService.UpdateUserAsync(
            existingUser.ApplicationUserId,
            new UpdateApplicationUserDto() { Email = request.Email }
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
