namespace Application.Dtos.User;

public record UserFiltersDto
{
    public string? FirstName { get; init; } = null;
    public string? LastName { get; init; } = null;
    public string? Email { get; init; } = null;
    public string? Role { get; init; } = null;
};
