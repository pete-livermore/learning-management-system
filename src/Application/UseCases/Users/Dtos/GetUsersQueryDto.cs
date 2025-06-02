namespace Application.UseCases.Users.Dtos;

public record GetUsersQueryDto
{
    public record PopulateDto { }

    public record FiltersDto
    {
        public string? FirstName { get; init; } = null;
        public string? LastName { get; init; } = null;
        public string? Email { get; init; } = null;
    }

    public FiltersDto Filters { get; init; } = new();
    public PopulateDto Populate { get; init; } = new();

    public int? Pages { get; init; }
    public int? Start { get; init; } = null;
}
