using Application.Common.Errors.Factories;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Security;
using Application.UseCases.Security.Interfaces;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Errors;
using Application.Wrappers.Results;
using Domain.Enums;
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
        var replaceUserDto = request.ReplaceCommand;
        var userEntity = await _usersRepository.FindByIdAsync(userId);

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

        var updateApplicationUserDto = new UpdateApplicationUserDto()
        {
            Email = replaceUserDto.Email,
        };
        var updateAplicationUserResult = await _identityService.UpdateUser(
            userEntity.ApplicationUserId,
            updateApplicationUserDto
        );

        if (updateAplicationUserResult.IsFailure)
        {
            return Result<UserDto>.Failure([.. updateAplicationUserResult.Errors]);
        }

        userEntity.Email = replaceUserDto.Email;
        userEntity.FirstName = replaceUserDto.FirstName;
        userEntity.LastName = replaceUserDto.LastName;
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
