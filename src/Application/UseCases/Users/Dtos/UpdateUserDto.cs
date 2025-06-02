namespace Application.UseCases.Users.Dtos
{
    using System.ComponentModel.DataAnnotations;

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
