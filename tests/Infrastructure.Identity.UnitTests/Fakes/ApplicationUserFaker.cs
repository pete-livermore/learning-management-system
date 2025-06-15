using Bogus;
using Infrastructure.Identity.Models;

namespace Infrastructure.Identity.Fakes;

public static class ApplicationUserFaker
{
    static ApplicationUserFaker()
    {
        Randomizer.Seed = new Random(42);
    }

    public static readonly ApplicationUser Fake = new Faker<ApplicationUser>()
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.PasswordHash, f => f.Internet.Password());
}
