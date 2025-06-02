using Application.Common.Dtos;
using Application.Common.Interfaces.Repositories;
using Application.UseCases.Users.Dtos;
using Application.Wrappers.Results;
using MediatR;

namespace Application.UseCases.Users.Queries;

public record class GetUsersQuery : IRequest<Result<PaginatedList<UserDto>>>
{
    public UserFiltersDto? Filters { get; init; }
    public PaginationParamsDto? Pagination { get; init; }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PaginatedList<UserDto>>>
{
    private readonly IUsersRepository _usersRepository;

    public GetUsersQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Result<PaginatedList<UserDto>>> Handle(
        GetUsersQuery query,
        CancellationToken cancellationToken
    )
    {
        var filters = query.Filters;
        var pagination = query.Pagination;

        var (users, totalPages) = await _usersRepository.FindMany(filters, pagination);
        var userDtos = users
            .Select(user => new UserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString(),
            })
            .ToList();

        int pageIndex = pagination?.PageIndex ?? 1;
        var usersList = new PaginatedList<UserDto>(userDtos, pageIndex, totalPages);

        return Result<PaginatedList<UserDto>>.Success(usersList);
    }
}
