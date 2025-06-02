using System.Security.Claims;

namespace Infrastructure.Identity.Extensions
{
    /// <summary>
    /// Claims related extensions for <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public static class PrincipalExtensions
    {
        /// <summary>
        /// Returns the User Name claim value if present otherwise returns null.
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance this method extends.</param>
        /// <returns>The Name claim value, or null if the claim is not present.</returns>
        /// <remarks>The name claim is identified by <see cref="ClaimsIdentity.DefaultNameClaimType"/>.</remarks>
        public static string? GetUserName(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirstValue(ClaimsIdentity.DefaultNameClaimType);
        }

        /// <summary>
        /// Returns the User ID claim value if present otherwise returns null.
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance this method extends.</param>
        /// <returns>The User ID claim value, or null if the claim is not present.</returns>
        /// <remarks>The name claim is identified by <see cref="ClaimTypes.NameIdentifier"/>.</remarks>
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string? GetUserRole(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirstValue(ClaimTypes.Role);
        }
    }
}
