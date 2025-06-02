using Application.Dtos.Auth;
using Application.Interfaces.Auth;
using Domain.Enums;
using Infrastructure.Identity.Extensions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Identity.Services;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public AuthenticatedUserDto GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var userId = user.GetUserId();
        var userRole = user.GetUserRole();

        if (userId == null)
        {
            throw new InvalidOperationException("User ID claim is missing.");
        }

        if (userRole == null)
        {
            throw new InvalidOperationException("User role claim is missing.");
        }

        return new AuthenticatedUserDto()
        {
            Id = int.Parse(userId),
            Role = Enum.Parse<UserRole>(userRole),
        };
    }
}
