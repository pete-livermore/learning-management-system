using Application.Common.Interfaces.Repositories;
using Application.UseCases.Security.Errors;
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
    private readonly IUserAccessor _userAccessor;

    public GetUserByIdQueryHandler(IUsersRepository usersRepository, IUserAccessor userAccessor)
    {
        _usersRepository = usersRepository;
        _userAccessor = userAccessor;
    }

    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = request.UserId;
        var currentUser = _userAccessor.GetCurrentUser();

        if (currentUser.Role != UserRole.Admin && currentUser.Id != userId)
        {
            return Result<UserDto>.Failure(SecurityErrors.Forbidden());
        }

        var user = await _usersRepository.FindById(userId);

        if (user is null)
        {
            return Result<UserDto>.Failure(UserErrors.NotFound(userId));
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
