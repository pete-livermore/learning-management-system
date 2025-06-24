using Application.Common.Interfaces.Repositories;
using Application.Common.Wrappers.Results;
using Application.Security.Dtos;
using Application.Security.Interfaces;
using Application.Users.Dtos;
using Application.Users.Errors;
using Domain.Users.Enums;
using MediatR;

namespace Application.Users.Commands;

public record class ReplaceUserCommand : IRequest<Result<UserDto>>
{
    public required int UserId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Role { get; init; }
}

public class ReplaceUserCommandHandler : IRequestHandler<ReplaceUserCommand, Result<UserDto>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IIdentityService _identityService;

    public ReplaceUserCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        IIdentityService identityService
    )
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _identityService = identityService;
    }

    public async Task<Result<UserDto>> Handle(
        ReplaceUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = request.UserId;
        var userEntity = await _usersRepository.FindByIdAsync(userId, cancellationToken);

        if (userEntity is null)
        {
            return Result<UserDto>.Failure(UserErrors.NotFound(userId));
        }

        var currentUserDto = _currentUserProvider.GetCurrentUser();
        var isAdminUser = currentUserDto.Roles.Any(r => r == UserRole.Administrator);
        if (!isAdminUser && userEntity.ApplicationUserId != currentUserDto.Id)
        {
            return Result<UserDto>.Failure(UserErrors.Forbidden());
        }

        var updateApplicationUserDto = new UpdateApplicationUserDto() { Email = request.Email };
        var updateAplicationUserResult = await _identityService.UpdateUserAsync(
            userEntity.ApplicationUserId,
            updateApplicationUserDto
        );

        if (updateAplicationUserResult.IsFailure)
        {
            return Result<UserDto>.Failure([.. updateAplicationUserResult.Errors]);
        }

        userEntity.Email = request.Email;
        userEntity.FirstName = request.FirstName;
        userEntity.LastName = request.LastName;
        userEntity.Role = userEntity.Role;

        _usersRepository.Update(userEntity);

        var updatedUserDto = new UserDto()
        {
            Id = userEntity.Id,
            FirstName = userEntity.FirstName,
            LastName = userEntity.LastName,
            Email = userEntity.Email,
            Role = userEntity.Role.ToString(),
        };

        var commitResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (commitResult.IsFailure)
        {
            return Result<UserDto>.Failure([.. commitResult.Errors]);
        }

        return Result<UserDto>.Success(updatedUserDto);
    }
}
