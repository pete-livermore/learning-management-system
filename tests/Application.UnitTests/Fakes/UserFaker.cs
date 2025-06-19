using Bogus;
using Domain.Users.Entities;

namespace Application.UnitTests.Fakes;

public static class UserFaker
{
    public static User Create() =>
        new Faker<User>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .Generate();
}
