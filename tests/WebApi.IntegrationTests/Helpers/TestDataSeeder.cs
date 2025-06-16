using System.Text.Json;
using Application.Common.Interfaces.Security;
using Application.UseCases.Security.Dtos;
using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Dtos;
using Domain.Enums;
using MediatR;
using WebApi.IntegrationTests.Fixtures;

namespace WebApi.IntegrationTests.Helpers;

public class TestDataSeeder
{
    private readonly ILogger<TestDataSeeder> _logger;
    private readonly IIdentityService _identityService;
    private readonly IMediator _mediator;

    public TestDataSeeder(
        IMediator mediator,
        IIdentityService identityService,
        ILogger<TestDataSeeder> logger
    )
    {
        _mediator = mediator;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task SeedRoles()
    {
        var role = new CreateApplicationRoleDto() { Name = UserRole.Administrator.ToString() };
        var createResult = await _identityService.CreateRoleAsync(role);

        if (createResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Seeding roles failed: {createResult.Errors[0].Description}"
            );
        }
    }

    public async Task SeedUsers()
    {
        var testUserDto = TestUserFixture.Admin;
        Console.WriteLine($"USER TO SEED => {JsonSerializer.Serialize(testUserDto)}");
        var userEmail = testUserDto.Email;
        var existingUser = await _identityService.FindUserByEmailAsync(userEmail);

        if (existingUser is not null)
        {
            _logger.LogInformation("Application user already exists");
        }

        var createUserDto = new CreateUserDto()
        {
            Email = testUserDto.Email,
            FirstName = testUserDto.FirstName,
            LastName = testUserDto.LastName,
            Password = testUserDto.Password,
            Role = testUserDto.Role.ToString(),
        };
        var createUserCommand = new CreateUserCommand() { CreateCommand = createUserDto };
        var result = await _mediator.Send(createUserCommand);

        if (result.IsFailure)
        {
            throw new Exception($"User seeding failed: {result.Errors[0].Description}");
        }
    }
}
