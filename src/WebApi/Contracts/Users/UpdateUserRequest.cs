using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Users;

public record class UpdateUserRequest
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }

    [EmailAddress]
    public string? Email { get; init; }

    [DataType(DataType.Password)]
    public string? Password { get; init; }
}
