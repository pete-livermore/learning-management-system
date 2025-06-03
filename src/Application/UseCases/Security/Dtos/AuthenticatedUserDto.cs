using Domain.Enums;

namespace Application.UseCases.Security.Dtos;

public record AuthenticatedUserDto
{
    public int Id { get; init; }
    public UserRole Role { get; init; }
}
