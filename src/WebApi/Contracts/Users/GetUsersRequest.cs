namespace WebApi.Contracts.Users;

public record class GetUsersRequest
{
    private record Populate { }

    public record UserFilters
    {
        public string? FirstName { get; init; } = null;
        public string? LastName { get; init; } = null;
        public string? Email { get; init; } = null;
    }

    public UserFilters? Filters { get; init; }
    public int? Start { get; init; } = 1;
    public int? Pages { get; init; } = 30;
}
