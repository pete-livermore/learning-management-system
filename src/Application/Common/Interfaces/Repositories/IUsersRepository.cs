using Application.Common.Dtos;
using Application.Users.Dtos;
using Domain.Users.Entities;

namespace Application.Common.Interfaces.Repositories;

public interface IUsersRepository
{
    void Add(User user);
    Task<User?> FindByIdAsync(int id);
    Task<User?> FindByEmailAsync(string email);
    Task<(List<User>, int totalPages)> FindManyAsync(
        UserFiltersDto? filters = null,
        PaginationParamsDto? pagination = null
    );
    void Update(User userToUpdate);
    void Delete(User userToDelete);
};
