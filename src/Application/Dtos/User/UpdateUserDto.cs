using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.User
{
    public record UpdateUserDto
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }

        [EmailAddress]
        public string? Email { get; init; }

        [DataType(DataType.Password)]
        public string? Password { get; init; }
    }
}
