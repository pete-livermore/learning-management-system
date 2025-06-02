using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Auth
{
    public record LoginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; init; }
    }
}
