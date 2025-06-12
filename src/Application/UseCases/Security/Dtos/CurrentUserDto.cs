using Domain.Enums;

namespace Application.UseCases.Security.Dtos;

public record CurrentUserDto
{
    public Guid Id { get; init; }
    public required IReadOnlyList<UserRole> Roles { get; init; }
}
