using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Users;

public record CreateUserRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    [EmailAddress]
    public required string Email { get; init; }

    [DataType(DataType.Password)]
    public required string Password { get; init; }
    public required string Role { get; set; }
}
