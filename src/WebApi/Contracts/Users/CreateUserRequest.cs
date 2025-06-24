using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Users;

public record CreateUserRequest
{
    [Required]
    public required string FirstName { get; init; }

    [Required]
    public required string LastName { get; init; }

    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; init; }

    [Required]
    public required string Role { get; set; }
}
