using Application.Common.Errors;
using Application.Common.Errors.Factories;
using Application.Common.Interfaces.Repositories;
using Application.UseCases.Security.Dtos;
using Application.UseCases.Security.Interfaces;
using Application.UseCases.Users.Queries;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.UnitTests.UseCases.Users.Queries
{
    public class GetUserByIdQueryTests
    {
        private readonly Mock<IUsersRepository> _repositoryMock;
        private readonly Mock<ICurrentUserProvider> _currentUserProviderMock;

        public GetUserByIdQueryTests()
        {
            _repositoryMock = new Mock<IUsersRepository>();
            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
        }

        private GetUserByIdQueryHandler CreateHandler()
        {
            return new GetUserByIdQueryHandler(
                _currentUserProviderMock.Object,
                _repositoryMock.Object
            );
        }

        [Fact]
        public async void ShouldReturnForbidden_WhenUserIsNotAuthorised()
        {
            var handler = CreateHandler();
            var currentUserDto = new CurrentUserDto()
            {
                Id = Guid.NewGuid(),
                Roles = [UserRole.Learner],
            };
            var domainUser = new User()
            {
                Id = 267,
                ApplicationUserId = Guid.NewGuid(),
                Email = "terest@ghghkl.com",
                FirstName = "First",
                LastName = "Last",
                Role = UserRole.Learner,
            };
            _currentUserProviderMock.Setup(cup => cup.GetCurrentUser()).Returns(currentUserDto);
            _repositoryMock.Setup(rp => rp.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(domainUser);
            var getUserByIdQuery = new GetUserByIdQuery() { UserId = 12 };

            var result = await handler.Handle(getUserByIdQuery, new CancellationToken());

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.Security, result.Errors[0].Type);
            Assert.Equal(SecurityError.ForbiddenCode, result.Errors[0].Code);
        }

        [Fact]
        public async void ShouldReturnNotFound_WhenUserIdDoesntMatchUser()
        {
            var handler = CreateHandler();
            var userIdToQuery = 78;
            var currentUserDto = new CurrentUserDto()
            {
                Id = Guid.NewGuid(),
                Roles = [UserRole.Administrator],
            };
            _currentUserProviderMock.Setup(ua => ua.GetCurrentUser()).Returns(currentUserDto);

            var getUserByIdQuery = new GetUserByIdQuery() { UserId = userIdToQuery };

            _repositoryMock.Setup(ur => ur.FindByIdAsync(userIdToQuery)).ReturnsAsync((User?)null);

            var result = await handler.Handle(getUserByIdQuery, new CancellationToken());

            _repositoryMock.Verify(r => r.FindByIdAsync(userIdToQuery), Times.Once);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.ResourcePersistence, result.Errors[0].Type);
            Assert.Equal(ResourceError.NotFoundCode, result.Errors[0].Code);
        }

        [Fact]
        public async void ShouldReturnSuccess_WhenUserIsSuccessfullyRetieved()
        {
            var handler = CreateHandler();
            var userIdToQuery = 78;
            var applicationUserId = Guid.NewGuid();
            var currentUserDto = new CurrentUserDto()
            {
                Id = applicationUserId,
                Roles = [UserRole.Learner],
            };
            _currentUserProviderMock.Setup(ua => ua.GetCurrentUser()).Returns(currentUserDto);

            var getUserByIdQuery = new GetUserByIdQuery() { UserId = userIdToQuery };

            _repositoryMock
                .Setup(ur => ur.FindByIdAsync(userIdToQuery))
                .ReturnsAsync(
                    new User()
                    {
                        Id = userIdToQuery,
                        FirstName = "test",
                        LastName = "person",
                        ApplicationUserId = applicationUserId,
                        Email = "test.person@email.com",
                        Role = UserRole.Learner,
                    }
                );

            var result = await handler.Handle(getUserByIdQuery, new CancellationToken());

            _repositoryMock.Verify(r => r.FindByIdAsync(userIdToQuery), Times.Once);
            Assert.True(result.IsSuccess);
        }
    }
}
