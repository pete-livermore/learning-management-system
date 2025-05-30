using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.User
{
    public record ReplaceUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string FirstName { get; init; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string LastName { get; init; }

        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; init; }
    }
}
