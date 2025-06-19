using System.Security.Claims;
using Application.Security.Dtos;
using Application.Security.Interfaces;
using Infrastructure.Identity.Extensions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Identity.Services;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUserDto GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var userId = user.GetClaimValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            throw new InvalidOperationException("User ID claim is missing.");
        }

        var userRole = user.GetClaimValue(ClaimTypes.Role);

        if (userRole == null)
        {
            throw new InvalidOperationException("User role claim is missing.");
        }

        var userRoles = user.GetUserRoles();

        return new CurrentUserDto() { Id = new Guid(userId), Roles = userRoles };
    }
}
