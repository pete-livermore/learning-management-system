namespace Application.Security.Dtos;

public record ApplicationUserDto
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required List<string> Roles { get; init; }
}
