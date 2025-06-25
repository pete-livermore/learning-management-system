using Application.Security.Dtos;

namespace Application.Security.Interfaces;

public interface ICurrentUserProvider
{
    /// <summary>
    /// Provides the current authenticated user based on the JWT identity claim
    /// </summary>
    /// <returns>
    /// A <see cref="CurrentUserDto"/> containing the authenticated user data
    /// </returns>
    CurrentUserDto GetCurrentUser();
}
