using System.Text;
using System.Text.Json;
using Application.UseCases.Security.Dtos;
using WebApi.IntegrationTests.Factories;
using WebApi.IntegrationTests.Fixtures;

namespace WebApi.IntegrationTests.Tests.Auth;

public class LoginTests
    : IClassFixture<SqlServerContainerFixture>,
        IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    private readonly SqlServerContainerFixture _dbFixture;
    private readonly CustomWebApplicationFactory<Startup> _apiFactory;

    public LoginTests(
        CustomWebApplicationFactory<Startup> apiFactory,
        SqlServerContainerFixture dbFixture
    )
    {
        _apiFactory = apiFactory;
        _dbFixture = dbFixture;

        _apiFactory.SetConnectionString(
            _dbFixture.DbConnectionFactory.DbConnection.ConnectionString
        );
        _client = _apiFactory.CreateClient();
    }

    [Fact]
    public async Task Login_ReturnsToken_WhenCredentialsAreValid()
    {
        var testDtoFixture = TestUserFixture.Admin;
        string testEmail = testDtoFixture.Email;
        string testPassword = testDtoFixture.Password;

        LoginDto loginDto = new() { Email = testEmail, Password = testPassword };
        var serializedLoginDto = JsonSerializer.Serialize(loginDto);
        var httpContent = new StringContent(serializedLoginDto, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/v1/auth/login", httpContent);

        await response.Content.ReadAsStringAsync();

        try
        {
            response.EnsureSuccessStatusCode();
            if (response.Content.Headers.ContentType is not null)
            {
                Assert.Equal(
                    "application/json; charset=utf-8",
                    response.Content.Headers.ContentType.ToString()
                );
            }
        }
        catch (HttpRequestException)
        {
            Assert.Fail(
                $"Expected a successful status code (2xx), but received: {response.StatusCode}"
            );
        }
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
    {
        var testDtoFixture = TestUserFixture.Admin;
        LoginDto loginDto = new() { Email = testDtoFixture.Email, Password = "incorrectpassword" };
        var httpContent = new StringContent(
            JsonSerializer.Serialize(loginDto),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _client.PostAsync("/v1/auth/login", httpContent);

        await response.Content.ReadAsStringAsync();
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
