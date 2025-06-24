using Application.Common.Interfaces.Repositories;
using Application.Common.Wrappers.Results;
using Application.Security.Interfaces;
using Application.Users.Errors;
using Domain.Users.Enums;
using MediatR;

namespace Application.Users.Commands.Delete;

public record class DeleteUserCommand : IRequest<Result>
{
    public required int UserId { get; init; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public DeleteUserCommandHandler(
        IUsersRepository usersRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider
    )
    {
        _usersRepository = usersRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var userToDelete = await _usersRepository.FindByIdAsync(userId);
        if (userToDelete is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }

        var currentUserDto = _currentUserProvider.GetCurrentUser();
        var isAdminUser = currentUserDto.Roles.Any(r => r == UserRole.Administrator);
        if (!isAdminUser && userToDelete.ApplicationUserId != currentUserDto.Id)
        {
            return Result.Failure(UserErrors.Forbidden());
        }

        var deleteIdentityUserResult = await _identityService.DeleteUserAsync(
            userToDelete.ApplicationUserId
        );
        if (deleteIdentityUserResult.IsFailure)
        {
            return Result.Failure([.. deleteIdentityUserResult.Errors]);
        }

        _usersRepository.Delete(userToDelete);

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            return Result.Failure([.. saveResult.Errors]);
        }

        return Result.Success();
    }
}
