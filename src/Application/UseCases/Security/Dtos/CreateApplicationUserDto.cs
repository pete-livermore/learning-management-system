namespace Application.UseCases.Security.Dtos;

public record class CreateApplicationUserDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required List<string> Roles { get; init; }
}
