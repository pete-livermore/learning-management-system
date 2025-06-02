using Application.Common.Dtos;
using Application.UseCases.Users.Dtos;
using Domain.Entities;

namespace Application.Common.Interfaces.Repositories;

public interface IUsersRepository
{
    Task<User> Add(User user);
    Task<User?> FindById(int id);
    Task<User?> FindByEmail(string email);
    Task<(List<User>, int totalPages)> FindMany(
        UserFiltersDto? filters = null,
        PaginationParamsDto? pagination = null
    );
    Task<User> Update(User userToUpdate);
    Task Delete(User userToDelete);
};
