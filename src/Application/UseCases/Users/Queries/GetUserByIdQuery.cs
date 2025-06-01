using System.Text.Json;
using Application.Dtos.User;
using Application.Errors;
using Application.Interfaces.Users;
using Application.Wrappers.Results;
using MediatR;

namespace Application.UseCases.Users.Queries;

public record GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public required int UserId { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUsersRepository _usersRepository;

    public GetUserByIdQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = request.UserId;
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
