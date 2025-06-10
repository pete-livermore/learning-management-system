using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Dtos;
using Domain.Enums;
using MediatR;

namespace WebApi.IntegrationTests.Helpers;

public class TestDataSeeder
{
    private readonly IMediator _mediator;

    public TestDataSeeder(IMediator mediator)
    {
        _mediator = mediator;
    }

    private async Task SeedUsers()
    {
        var createUserDto = new CreateUserDto()
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john_smith@email.com",
            Role = UserRole.User.ToString(),
            Password = "password",
        };
        var createUserCommand = new CreateUserCommand() { CreateCommand = createUserDto };
        var result = await _mediator.Send(createUserCommand);

        if (result.IsFailure)
        {
            throw new Exception("User seeding failed");
        }
    }

    public async Task SeedData()
    {
        await SeedUsers();
    }
}
