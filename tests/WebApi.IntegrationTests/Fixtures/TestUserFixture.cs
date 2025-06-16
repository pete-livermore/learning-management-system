using Domain.Enums;

namespace WebApi.IntegrationTests.Fixtures;

public sealed record TestUserDto
{
    public int Id { get; init; } = 1;
    public string FirstName { get; init; } = "Foo";
    public string LastName { get; init; } = "Bar";
    public string Email { get; init; } = "foo_bar@email.com";
    public string Password { get; init; } = "Secur3.P@ssw0rd";
    public required UserRole Role { get; init; }
    public required Guid ApplicationUserId { get; init; }
}

public static class TestUserFixture
{
    private static readonly TestUserDto _adminUser;

    static TestUserFixture()
    {
        _adminUser = new TestUserDto()
        {
            Role = UserRole.Administrator,
            ApplicationUserId = Guid.NewGuid(),
        };
    }

    public static TestUserDto Admin => _adminUser;
}
