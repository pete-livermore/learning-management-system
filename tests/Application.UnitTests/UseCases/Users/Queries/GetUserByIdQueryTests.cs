using Application.Common.Errors;
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
        private readonly Mock<IUserAccessor> _userAccessorMock;

        public GetUserByIdQueryTests()
        {
            _repositoryMock = new Mock<IUsersRepository>();
            _userAccessorMock = new Mock<IUserAccessor>();
        }

        private GetUserByIdQueryHandler CreateHandler()
        {
            return new GetUserByIdQueryHandler(_repositoryMock.Object, _userAccessorMock.Object);
        }

        [Fact]
        public async void ShouldReturnForbidden_WhenUserIsNotAuthorised()
        {
            var handler = CreateHandler();
            var authenticatedUser = new AuthenticatedUserDto() { Id = 346, Role = UserRole.User };
            var getUserByIdQuery = new GetUserByIdQuery() { UserId = 12 };

            _userAccessorMock.Setup(ua => ua.GetCurrentUser()).Returns(authenticatedUser);

            var result = await handler.Handle(getUserByIdQuery, new CancellationToken());

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.Forbidden, result.Errors[0].Type);
        }

        [Fact]
        public async void ShouldReturnNotFound_WhenUserIdDoesntMatchUser()
        {
            var handler = CreateHandler();
            var userIdToQuery = 78;
            var authenticatedUser = new AuthenticatedUserDto() { Id = 200, Role = UserRole.Admin };
            var getUserByIdQuery = new GetUserByIdQuery() { UserId = userIdToQuery };

            _userAccessorMock.Setup(ua => ua.GetCurrentUser()).Returns(authenticatedUser);
            _repositoryMock.Setup(ur => ur.FindById(userIdToQuery)).ReturnsAsync((User?)null);

            var result = await handler.Handle(getUserByIdQuery, new CancellationToken());

            _repositoryMock.Verify(r => r.FindById(userIdToQuery), Times.Once);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.NotFound, result.Errors[0].Type);
        }

        [Fact]
        public async void ShouldReturnSuccess_WhenUserIsSuccessfullyRetieved()
        {
            var handler = CreateHandler();
            var userIdToQuery = 78;
            var authenticatedUser = new AuthenticatedUserDto()
            {
                Id = userIdToQuery,
                Role = UserRole.User,
            };
            var getUserByIdQuery = new GetUserByIdQuery() { UserId = userIdToQuery };

            _userAccessorMock.Setup(ua => ua.GetCurrentUser()).Returns(authenticatedUser);
            _repositoryMock
                .Setup(ur => ur.FindById(userIdToQuery))
                .ReturnsAsync(
                    new User()
                    {
                        Id = userIdToQuery,
                        FirstName = "test",
                        LastName = "person",
                        Password = "P@ssw0rd",
                        Email = "test.person@email.com",
                        Role = UserRole.User,
                    }
                );

            var result = await handler.Handle(getUserByIdQuery, new CancellationToken());

            _repositoryMock.Verify(r => r.FindById(userIdToQuery), Times.Once);
            Assert.True(result.IsSuccess);
        }
    }
}
