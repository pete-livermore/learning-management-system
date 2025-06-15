using System.Threading.Tasks;
using Application.Common.Errors;
using Application.Common.Errors.Factories;
using Application.Common.Interfaces.Security;
using Application.UseCases.Security.Dtos;
using Bogus;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Services;
using Infrastructure.Identity.UnitTests.Mocks;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Infrastructure.Identity.UnitTests.Tests.Services;

public class IdentityServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock =
        UserManagerMock.GetMock<ApplicationUser>([]);
    private readonly Mock<RoleManager<ApplicationRole>> _roleManagerMock =
        RoleManagerMock.GetMock<ApplicationRole>();
    private readonly Mock<TokenService> _tokenServiceMock = TokenServiceMock.GetMock();

    private IIdentityService CreateIdentityService()
    {
        return new IdentityService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _tokenServiceMock.Object
        );
    }

    [Fact]
    public async Task ShouldReturnConflictFailure_WhenApplicationUserEmailAlreadyExists()
    {
        var identityService = CreateIdentityService();

        var failureResult = IdentityResult.Failed(new IdentityError() { Code = "DuplicateEmail" });
        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(failureResult);
        var createApplicationUserDto = new Faker<CreateApplicationUserDto>();

        var result = await identityService.CreateUserAsync(createApplicationUserDto);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.ResourcePersistence, result.Errors[0].Type);
        Assert.Equal(ResourceError.ConflictCode, result.Errors[0].Code);
    }
}
