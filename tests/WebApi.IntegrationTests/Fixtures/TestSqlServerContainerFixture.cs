using Testcontainers.MsSql;
using WebApi.IntegrationTests.Factories;

namespace WebApi.IntegrationTests.Fixtures
{
    public interface IDatabaseContainerFixture
    {
        MsSqlContainer MssqlContainer { get; }
    }

    public sealed class SqlServerContainerFixture : IAsyncLifetime
    {
        private const string Database = "TestDatabase";
        private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
        public DbConnectionFactory DbConnectionFactory;

        public SqlServerContainerFixture()
        {
            DbConnectionFactory = new DbConnectionFactory(_msSqlContainer, Database);
        }

        public async Task InitializeAsync()
        {
            // Start the container asynchronously
            await _msSqlContainer.StartAsync();
            Console.WriteLine(
                $"SQL Server container is starting on {_msSqlContainer.GetConnectionString()}"
            );
        }

        public Task DisposeAsync()
        {
            // Dispose of the container asynchronously
            return _msSqlContainer.DisposeAsync().AsTask();
        }
    }
}
