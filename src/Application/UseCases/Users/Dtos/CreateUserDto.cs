using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.UseCases.Users.Dtos
{
    public record CreateUserDto
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
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters long."
        )]
        public required string Password { get; init; }

        [EnumDataType(typeof(UserRole))]
        public required UserRole Role { get; set; }
    }
}
