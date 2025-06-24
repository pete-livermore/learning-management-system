using Application.Common.Dtos;
using Application.Users.Dtos;
using Domain.Users.Entities;

namespace Application.Common.Interfaces.Repositories;

public interface IUsersRepository
{
    void Add(User user);
    Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken);
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<(List<User>, int totalPages)> FindManyAsync(
        UserFiltersDto? filters,
        PaginationParamsDto? pagination,
        CancellationToken cancellationToken
    );
    void Update(User userToUpdate);
    void Delete(User userToDelete);
};
