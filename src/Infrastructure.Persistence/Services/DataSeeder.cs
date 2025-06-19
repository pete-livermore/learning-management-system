using Application.Security.Dtos;
using Application.Security.Interfaces;
using Application.Users.Commands;
using Domain.Users.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Helpers;

public sealed record UserSeedDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required UserRole Role { get; init; }
}

public class DataSeeder
{
    private readonly ILogger<DataSeeder> _logger;
    private readonly IIdentityService _identityService;
    private readonly IMediator _mediator;

    public DataSeeder(
        IMediator mediator,
        IIdentityService identityService,
        ILogger<DataSeeder> logger
    )
    {
        _mediator = mediator;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task SeedApplicationRoleAsync(UserRole userRole)
    {
        var role = new CreateApplicationRoleDto() { Name = userRole.ToString() };
        var createResult = await _identityService.CreateRoleAsync(role);

        if (createResult.IsFailure)
        {
            var failureMessage = createResult.Errors[0];
            _logger.LogError("Roles seeding failure: {failureMessage}", failureMessage);
            throw new InvalidOperationException("Error seeding test application roles");
        }
    }

    public async Task SeedUserAsync(UserSeedDto userSeedDto)
    {
        var userEmail = userSeedDto.Email;
        var existingUser = await _identityService.FindUserByEmailAsync(userEmail);

        if (existingUser is not null)
        {
            _logger.LogInformation(
                "Application user with email {userEmail} already exists",
                userEmail
            );
            return;
        }

        var createUserCommand = new CreateUserCommand()
        {
            Email = userSeedDto.Email,
            FirstName = userSeedDto.FirstName,
            LastName = userSeedDto.LastName,
            Password = userSeedDto.Password,
            Role = userSeedDto.Role.ToString(),
        };
        var result = await _mediator.Send(createUserCommand);

        if (result.IsFailure)
        {
            var failureMessage = result.Errors[0].Description;
            _logger.LogError("User seeding failure: {failureMessage}", failureMessage);
            throw new Exception($"Error seeding users");
        }
    }
}
