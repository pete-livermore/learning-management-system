using Application.Dtos;
using Application.Dtos.User;
using Domain.Entities;

namespace Application.Interfaces.Users;

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
