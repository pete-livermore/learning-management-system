using Domain.Enums;
using Domain.Users.Enums;

namespace WebApi.IntegrationTests.Fixtures;

public sealed record TestUserDto
{
    public string FirstName { get; init; } = "Foo";
    public string LastName { get; init; } = "Bar";
    public string Email { get; init; } = "foo_bar@email.com";
    public string Password { get; init; } = "Secur3.P@ssw0rd";
    public required UserRole Role { get; init; }
}

public static class TestUserFixture
{
    private static readonly TestUserDto _adminUser;

    static TestUserFixture()
    {
        _adminUser = new TestUserDto() { Role = UserRole.Administrator };
    }

    public static TestUserDto Admin => _adminUser;
}
