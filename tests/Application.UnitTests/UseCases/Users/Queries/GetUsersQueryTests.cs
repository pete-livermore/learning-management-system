using Application.Common.Dtos;
using Application.Common.Errors;
using Application.Common.Errors.Factories;
using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Repositories;
using Application.UseCases.Security.Dtos;
using Application.UseCases.Security.Interfaces;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Queries;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.UnitTests.UseCases.Users.Queries
{
    public class GetUsersQueryTests
    {
        private readonly Mock<IUsersRepository> _usersRepositoryMock = new();
        private readonly Mock<ICurrentUserProvider> _currentUserProviderMock = new();
        private readonly Mock<ICacheService> _cacheServiceMock = new();

        public GetUsersQueryHandler CreateHandler()
        {
            return new GetUsersQueryHandler(
                _usersRepositoryMock.Object,
                _currentUserProviderMock.Object,
                _cacheServiceMock.Object
            );
        }

        [Fact]
        public async Task ShouldReturnForbiddenError_WhenUserIsNotAdmin()
        {
            var handler = CreateHandler();
            var currentUserDto = new CurrentUserDto()
            {
                Id = Guid.NewGuid(),
                Roles = [UserRole.Learner],
            };
            _currentUserProviderMock.Setup(cup => cup.GetCurrentUser()).Returns(currentUserDto);

            var testQuery = new GetUsersQuery() { };
            var result = await handler.Handle(testQuery, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.Security, result.Errors[0].Type);
            Assert.Equal(SecurityError.ForbiddenCode, result.Errors[0].Code);
        }

        [Fact]
        public async Task ShouldReturnCachedUsers_WhenCachedUsersExist()
        {
            var currentUserDto = new CurrentUserDto()
            {
                Id = new Guid(),
                Roles = [UserRole.Administrator],
            };
            _currentUserProviderMock.Setup(cup => cup.GetCurrentUser()).Returns(currentUserDto);
            var handler = CreateHandler();
            List<UserDto> cachedUsers =
            [
                new UserDto()
                {
                    Id = 25,
                    Email = "r@ndom_user@domain.org",
                    FirstName = "Random",
                    LastName = "User",
                    Role = UserRole.Learner.ToString(),
                },
            ];
            var cachedUsersList = new PaginatedList<UserDto>(cachedUsers, 0, 1);

            _cacheServiceMock
                .Setup(cs =>
                    cs.GetValueAsync<PaginatedList<UserDto>>(
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(cachedUsersList);

            var query = new GetUsersQuery();

            var result = await handler.Handle(query, CancellationToken.None);

            _usersRepositoryMock.Verify(
                ur => ur.FindManyAsync(It.IsAny<UserFiltersDto>(), It.IsAny<PaginationParamsDto>()),
                Times.Never
            );

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ShouldRetriveUsersFromRepository_WhenCachedUsersDontExist()
        {
            var currentUserDto = new CurrentUserDto()
            {
                Id = new Guid(),
                Roles = [UserRole.Administrator],
            };
            _currentUserProviderMock.Setup(cup => cup.GetCurrentUser()).Returns(currentUserDto);
            var handler = CreateHandler();
            List<User> usersList =
            [
                new User()
                {
                    Id = 25,
                    Email = "r@ndom_user@domain.org",
                    FirstName = "Random",
                    LastName = "User",
                    ApplicationUserId = new Guid(),
                    Role = UserRole.Learner,
                },
            ];
            _usersRepositoryMock
                .Setup(ur =>
                    ur.FindManyAsync(It.IsAny<UserFiltersDto>(), It.IsAny<PaginationParamsDto>())
                )
                .ReturnsAsync((usersList, 1));

            var getUsersQuery = new GetUsersQuery();

            var result = await handler.Handle(getUsersQuery, CancellationToken.None);

            _usersRepositoryMock.Verify(
                ur => ur.FindManyAsync(It.IsAny<UserFiltersDto>(), It.IsAny<PaginationParamsDto>()),
                Times.Once
            );

            Assert.True(result.IsSuccess);
        }
    }
}
