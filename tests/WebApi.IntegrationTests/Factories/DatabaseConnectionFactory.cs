using System.Data.Common;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;

namespace WebApi.IntegrationTests.Factories;

public sealed class DbConnectionFactory
{
    private readonly IDatabaseContainer _databaseContainer;
    private readonly string _database;

    public DbConnectionFactory(IDatabaseContainer databaseContainer, string database)
    {
        _databaseContainer = databaseContainer;
        _database = database;
    }

    public DbConnection DbConnection
    {
        get
        {
            var connectionString = new SqlConnectionStringBuilder(
                _databaseContainer.GetConnectionString()
            );
            connectionString.InitialCatalog = _database;
            return new SqlConnection(connectionString.ToString());
        }
    }
}
