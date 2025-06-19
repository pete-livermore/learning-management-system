using Application.Common.Errors;
using Application.Common.Errors.Factories;
using Application.Common.Interfaces.Repositories;
using Application.Common.Wrappers.Results;
using Application.Security.Dtos;
using Application.Security.Interfaces;
using Application.Users.Commands;
using Domain.Users.Entities;
using Domain.Users.Enums;
using Moq;

namespace Application.UnitTests.Tests.Users.Commands;

public class CreateUserCommandTests
{
    private readonly Mock<IUsersRepository> _repositoryMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private CreateUserCommandHandler CreateHandler()
    {
        return new CreateUserCommandHandler(
            _identityServiceMock.Object,
            _repositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenUserAlreadyExists()
    {
        var handler = CreateHandler();
        string testEmail = "test_email@email.com";
        string testPassword = "password";

        var applicationUserDto = new ApplicationUserDto()
        {
            Id = Guid.NewGuid(),
            Email = testEmail,
            Roles = [UserRole.Administrator.ToString()],
        };

        var successResult = Result<ApplicationUserDto>.Success(applicationUserDto);

        _identityServiceMock
            .Setup(i => i.CreateUserAsync(It.IsAny<CreateApplicationUserDto>()))
            .ReturnsAsync(successResult);

        var command = new CreateUserCommand()
        {
            FirstName = "test",
            LastName = "user",
            Email = testEmail,
            Password = testPassword,
            Role = UserRole.Administrator.ToString(),
        };

        _repositoryMock
            .Setup(ur => ur.FindByEmailAsync(command.Email))
            .ReturnsAsync(
                new User
                {
                    Email = command.Email,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    ApplicationUserId = Guid.NewGuid(),
                    Role = UserRole.Learner,
                }
            );

        var result = await handler.Handle(command, new CancellationToken());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.ResourcePersistence, result.Errors[0].Type);
        Assert.Equal(ResourceError.ConflictCode, result.Errors[0].Code);
    }

    [Fact]
    public async Task ShouldReturnSuccess_WhenUserIsValid()
    {
        var handler = CreateHandler();
        string testCurrentUserEmail = "test_email@email.com";
        var applicationUserId = Guid.NewGuid();

        var applicationUserDto = new ApplicationUserDto()
        {
            Id = applicationUserId,
            Email = testCurrentUserEmail,
            Roles = [UserRole.Administrator.ToString()],
        };

        var successResult = Result<ApplicationUserDto>.Success(applicationUserDto);

        _identityServiceMock
            .Setup(i => i.CreateUserAsync(It.IsAny<CreateApplicationUserDto>()))
            .ReturnsAsync(successResult);

        string testEmail = "test@example.com";
        string testPassword = "password";
        string testFirstName = "Test";
        string testLastName = "User";
        var testRole = UserRole.Learner;

        var newUser = new User
        {
            Email = testEmail,
            FirstName = testFirstName,
            LastName = testLastName,
            Role = testRole,
            ApplicationUserId = applicationUserId,
        };

        _repositoryMock.Setup(repo => repo.FindByEmailAsync(testEmail)).ReturnsAsync((User?)null);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<int>.Success(1));

        var testCommand = new CreateUserCommand()
        {
            Email = testEmail,
            Password = testPassword,
            FirstName = testFirstName,
            LastName = testLastName,
            Role = testRole.ToString(),
        };
        var result = await handler.Handle(testCommand, new CancellationToken());

        _repositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
        Assert.True(result.IsSuccess);
    }
}
