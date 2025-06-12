using System.Security.Claims;
using Domain.Enums;

namespace Infrastructure.Identity.Extensions
{
    /// <summary>
    /// Claims related extensions for <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public static class PrincipalExtensions
    {
        public static string? GetClaimValue(this ClaimsPrincipal principal, string claimType)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirstValue(claimType);
        }

        public static IReadOnlyList<UserRole> GetUserRoles(this ClaimsPrincipal principal)
        {
            List<UserRole> domainRoles = new();
            List<string> identityRoles = principal
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            foreach (var role in identityRoles)
            {
                if (Enum.TryParse<UserRole>(role, out var result))
                {
                    domainRoles.Add(result);
                }
                ;
            }
            return domainRoles.AsReadOnly();
        }
    }
}
