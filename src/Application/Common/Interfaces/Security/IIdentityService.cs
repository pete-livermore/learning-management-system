using Application.UseCases.Security.Dtos;
using Application.UseCases.Users.Dtos;
using Application.Wrappers.Results;

namespace Application.Common.Interfaces.Security
{
    public interface IIdentityService
    {
        Task<Result<ApplicationUserDto>> CreateUserAsync(CreateApplicationUserDto createUserDto);
        Task<Result<List<string>>> GetUserRolesAsync(Guid userId);
        Task<Result<string>> AuthenticateUserAsync(string userEmail, string password);
        Task<Result> UpdateUser(Guid userId, UpdateApplicationUserDto updateApplicationUserDto);
        Task<Result> DeleteAsync(Guid userId);
    }
}
