using Domain.Enums;

namespace Application.UseCases.Users.Dtos
{
    public record CreateUserDto
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string Role { get; set; }
    }
}
