using Application.Common.Wrappers.Results;
using Application.Security.Dtos;

namespace Application.Security.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<ApplicationUserDto>> CreateUserAsync(CreateApplicationUserDto createUserDto);
        Task<Result<ApplicationUserDto>> FindUserByIdAsync(Guid userId);
        Task<Result<ApplicationUserDto>> FindUserByEmailAsync(string userEmail);
        Task<Result> UpdateUserAsync(
            Guid userId,
            UpdateApplicationUserDto updateApplicationUserDto
        );
        Task<Result> DeleteUserAsync(Guid userId);
        Task<Result<ApplicationRoleDto>> CreateRoleAsync(
            CreateApplicationRoleDto createApplicationRoleDto
        );
        Task<Result<List<string>>> GetUserRolesAsync(Guid userId);
        Task<Result<string>> AuthenticateUserAsync(string userEmail, string password);
    }
}
