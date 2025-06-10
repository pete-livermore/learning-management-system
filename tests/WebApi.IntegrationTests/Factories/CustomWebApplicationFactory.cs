using Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using WebApi.IntegrationTests.Helpers;

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
                var dbContext =
                    scope.ServiceProvider.GetRequiredService<LearningManagementSystemDbContext>();
                dbContext.Database.Migrate();

                // Database seeding
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var dataSeeder = new TestDataSeeder(mediator);
                dataSeeder.SeedData().GetAwaiter().GetResult();
            });

            builder.UseEnvironment("Development"); // Ensure consistent environment
        }
    }
}
