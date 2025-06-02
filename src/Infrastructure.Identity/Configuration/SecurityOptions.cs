using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace LearningManagementSystem.Infrastructure.Identity.Configuration
{
    public sealed class JwtOptions
    {
        public const string Jwt = "Jwt";

        [Required]
        public required string SecretKey { get; set; }

        [Required]
        public required string Issuer { get; set; }

        [Required]
        public required string Audience { get; set; }

        [Required]
        public required int ExpiryInMinutes { get; set; }
    }

    public class PasswordOptions
    {
        [Range(8, int.MaxValue, ErrorMessage = "The {0} must be greater than {1}.")]
        public int MinLength { get; set; } = 8;
    }

    public sealed class SecurityOptions
    {
        public const string Security = "Security";

        [ValidateObjectMembers]
        public required PasswordOptions Password { get; set; }

        [ValidateObjectMembers]
        public required JwtOptions Jwt { get; set; }
    }
}
