using Application.Common.Interfaces.Repositories;
using Application.UseCases.Security.Interfaces;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Errors;
using Application.Wrappers.Results;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Users.Queries;

public record GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public required int UserId { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetUserByIdQueryHandler(
        ICurrentUserProvider currentUserProvider,
        IUsersRepository usersRepository
    )
    {
        _currentUserProvider = currentUserProvider;
        _usersRepository = usersRepository;
    }

    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = request.UserId;
        var currentUser = _currentUserProvider.GetCurrentUser();

        var user = await _usersRepository.FindByIdAsync(userId);

        if (user is null)
        {
            return Result<UserDto>.Failure(UserErrors.NotFound(userId));
        }

        bool isAdminUser = currentUser.Roles.Contains(UserRole.Administrator);
        if (!isAdminUser && user.ApplicationUserId != currentUser.Id)
        {
            return Result<UserDto>.Failure(UserErrors.Forbidden());
        }

        var userDto = new UserDto()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role.ToString(),
        };

        return Result<UserDto>.Success(userDto);
    }
}
