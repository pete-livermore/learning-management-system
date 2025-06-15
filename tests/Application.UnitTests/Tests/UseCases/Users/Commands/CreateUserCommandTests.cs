using Application.Common.Errors;
using Application.Common.Errors.Factories;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Security;
using Application.UseCases.Security.Dtos;
using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Dtos;
using Application.Wrappers.Results;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.UnitTests.Tests.UseCases.Users.Commands;

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

        var testDto = new CreateUserDto()
        {
            FirstName = "test",
            LastName = "user",
            Email = testEmail,
            Password = testPassword,
            Role = UserRole.Administrator.ToString(),
        };

        var command = new CreateUserCommand() { CreateCommand = testDto };

        _repositoryMock
            .Setup(ur => ur.FindByEmailAsync(testDto.Email))
            .ReturnsAsync(
                new User
                {
                    Email = testDto.Email,
                    FirstName = testDto.FirstName,
                    LastName = testDto.LastName,
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
        string testEmail = "test_email@email.com";
        var applicationUserId = Guid.NewGuid();

        var applicationUserDto = new ApplicationUserDto()
        {
            Id = applicationUserId,
            Email = testEmail,
            Roles = [UserRole.Administrator.ToString()],
        };

        var successResult = Result<ApplicationUserDto>.Success(applicationUserDto);

        _identityServiceMock
            .Setup(i => i.CreateUserAsync(It.IsAny<CreateApplicationUserDto>()))
            .ReturnsAsync(successResult);

        var testDto = new CreateUserDto()
        {
            Email = "test@example.com",
            Password = "password",
            FirstName = "Test",
            LastName = "User",
            Role = UserRole.Learner.ToString(),
        };

        var newUser = new User
        {
            Email = testDto.Email,
            FirstName = testDto.FirstName,
            LastName = testDto.LastName,
            Role = UserRole.Learner,
            ApplicationUserId = applicationUserId,
        };

        _repositoryMock
            .Setup(repo => repo.FindByEmailAsync(testDto.Email))
            .ReturnsAsync((User?)null);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<int>.Success(1));

        var testCommand = new CreateUserCommand() { CreateCommand = testDto };
        var result = await handler.Handle(testCommand, new CancellationToken());

        _repositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
        Assert.True(result.IsSuccess);
    }
}
