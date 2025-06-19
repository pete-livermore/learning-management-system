using Application.Security.Interfaces;
using Domain.Users.Enums;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using WebApi.IntegrationTests.Fixtures;

namespace WebApi.IntegrationTests.Factories
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
        where TProgram : class
    {
        public string? ConnectionString { get; private set; }

        public void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private static async Task SeedTestDataAsync(IServiceProvider serviceProvider)
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();
            var logger = serviceProvider.GetRequiredService<ILogger<DataSeeder>>();
            var dataSeeder = new DataSeeder(mediator, identityService, logger);

            var seededUserRole = UserRole.Administrator;
            await dataSeeder.SeedApplicationRoleAsync(seededUserRole);

            var userFixture = TestUserFixture.Admin;
            var userSeedDto = new UserSeedDto()
            {
                Email = userFixture.Email,
                FirstName = userFixture.FirstName,
                LastName = userFixture.LastName,
                Password = userFixture.Password,
                Role = seededUserRole,
            };
            await dataSeeder.SeedUserAsync(userSeedDto);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(
                (context, config) => config.AddJsonFile("appsettings.Test.json", optional: true)
            );

            builder.ConfigureServices(services =>
            {
                // Find and remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<LearningManagementSystemDbContext>)
                );

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Register DbContext using the connection string set by the test class
                services.AddDbContext<LearningManagementSystemDbContext>(
                    (container, options) =>
                    {
                        if (string.IsNullOrEmpty(ConnectionString))
                        {
                            throw new InvalidOperationException(
                                "Test database connection string not set on WebApplicationFactory."
                            );
                        }
                        options.UseSqlServer(ConnectionString);
                    }
                );

                // Database creation/migration:
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var scopedServiceProvider = scope.ServiceProvider;
                var dbContext =
                    scopedServiceProvider.GetRequiredService<LearningManagementSystemDbContext>();
                dbContext.Database.Migrate();

                // Database seeding
                Task.Run(async () => await SeedTestDataAsync(scopedServiceProvider))
                    .GetAwaiter()
                    .GetResult();
            });

            builder.UseEnvironment("Development"); // Ensure consistent environment
        }
    }
}
