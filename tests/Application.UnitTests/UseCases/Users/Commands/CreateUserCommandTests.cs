using System.Threading.Tasks;
using Application.Common.Errors;
using Application.Common.Interfaces.Repositories;
using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Dtos;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands;

public class CreateUserCommandTests
{
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;

    public CreateUserCommandTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
    }

    private CreateUserCommandHandler CreateHandler()
    {
        return new CreateUserCommandHandler(_repositoryMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenUserAlreadyExists()
    {
        var handler = CreateHandler();

        var testDto = new CreateUserDto()
        {
            FirstName = "test",
            LastName = "user",
            Email = "test_email@email.com",
            Password = "password",
            Role = UserRole.Admin.ToString(),
        };

        var command = new CreateUserCommand() { CreateCommand = testDto };

        _repositoryMock
            .Setup(ur => ur.FindByEmail(testDto.Email))
            .ReturnsAsync(
                new User
                {
                    Email = testDto.Email,
                    FirstName = testDto.FirstName,
                    LastName = testDto.LastName,
                    Password = testDto.Password,
                    Role = UserRole.User,
                }
            );

        var result = await handler.Handle(command, new CancellationToken());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Errors[0].Type);
    }

    [Fact]
    public async Task ShouldReturnSuccess_WhenUserIsValid()
    {
        var handler = CreateHandler();

        var testDto = new CreateUserDto()
        {
            Email = "test@example.com",
            Password = "password",
            FirstName = "Test",
            LastName = "User",
            Role = UserRole.User.ToString(),
        };

        var newUser = new User
        {
            Email = testDto.Email,
            Password = testDto.Password,
            FirstName = testDto.FirstName,
            LastName = testDto.LastName,
            Role = UserRole.User,
        };

        var command = new CreateUserCommand() { CreateCommand = testDto };

        _repositoryMock.Setup(repo => repo.FindByEmail(testDto.Email)).ReturnsAsync((User?)null);

        _passwordHasherMock
            .Setup(ph => ph.HashPassword(It.IsAny<User>(), testDto.Password))
            .Returns("hashed_password");

        _repositoryMock.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(newUser);

        var result = await handler.Handle(command, new CancellationToken());

        _repositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
        Assert.True(result.IsSuccess);
    }
}
