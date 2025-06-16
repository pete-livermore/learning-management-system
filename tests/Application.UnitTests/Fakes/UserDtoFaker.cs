using Application.UseCases.Users.Dtos;
using Bogus;
using Domain.Enums;

namespace Application.UnitTests.Fakes;

public static class UserDtoFaker
{
    public static T Create<T>(UserRole userRole)
        where T : class
    {
        var type = typeof(T);

        Faker<T> faker;

        if (type == typeof(CreateUserDto))
        {
            faker =
                (Faker<T>)
                    (object)
                        new Faker<CreateUserDto>()
                            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                            .RuleFor(u => u.LastName, f => f.Name.LastName())
                            .RuleFor(u => u.Email, f => f.Internet.Email())
                            .RuleFor(u => u.Password, f => f.Internet.Password())
                            .RuleFor(u => u.Role, _ => userRole.ToString());
        }
        else
        {
            throw new ArgumentException($"Faker for type {type} not implemented.");
        }
        return faker.Generate();
    }
}
