using Domain.Users.Enums;

namespace Application.Security.Dtos;

public record CurrentUserDto
{
    public Guid Id { get; init; }
    public required IReadOnlyList<UserRole> Roles { get; init; }
}
