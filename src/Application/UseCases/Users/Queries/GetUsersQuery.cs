using System.Text.Json;
using Application.Common.Dtos;
using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Repositories;
using Application.UseCases.Security.Interfaces;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Errors;
using Application.Wrappers.Results;
using Domain.Enums;
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
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICacheService _cacheService;

    public GetUsersQueryHandler(
        IUsersRepository usersRepository,
        ICurrentUserProvider currentUserProvider,
        ICacheService cacheService
    )
    {
        _usersRepository = usersRepository;
        _currentUserProvider = currentUserProvider;
        _cacheService = cacheService;
    }

    public async Task<Result<PaginatedList<UserDto>>> Handle(
        GetUsersQuery query,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _currentUserProvider.GetCurrentUser();
        bool isAdminUser = currentUser.Roles.Contains(UserRole.Administrator);

        if (!isAdminUser)
        {
            return Result<PaginatedList<UserDto>>.Failure(UserErrors.Forbidden());
        }

        var filters = query.Filters;
        var pagination = query.Pagination;
        var filtersString = JsonSerializer.Serialize(filters);
        var cacheKey = $"Users_{filtersString}";
        var cachedUsersList = await _cacheService.GetValueAsync<PaginatedList<UserDto>>(
            cacheKey,
            cancellationToken
        );

        if (cachedUsersList is not null)
        {
            return Result<PaginatedList<UserDto>>.Success(cachedUsersList);
        }

        var (users, totalPages) = await _usersRepository.FindManyAsync(filters, pagination);
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

        await _cacheService.SetValueAsync(cacheKey, usersList);

        return Result<PaginatedList<UserDto>>.Success(usersList);
    }
}
