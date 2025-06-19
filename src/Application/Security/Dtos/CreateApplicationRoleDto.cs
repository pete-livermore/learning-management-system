namespace Application.Security.Dtos;

public record class CreateApplicationRoleDto
{
    public required string Name { get; init; }
}
