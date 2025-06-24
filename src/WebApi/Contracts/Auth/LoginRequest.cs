using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Auth;

public record class LoginRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }
}
