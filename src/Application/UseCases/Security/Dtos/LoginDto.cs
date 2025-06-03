using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.Security.Dtos
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
