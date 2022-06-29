using System.Data;
using Dapper;

namespace Infrastructure.Migrations;

public class Database
{
    private readonly DataContextFactory dataContextFactory;

    public Database(DataContextFactory dataContextFactory) => this.dataContextFactory = dataContextFactory;

    public void CreateDatabase(string databaseName, bool isForTests = false)
    {
        var query = "SELECT datname FROM pg_catalog.pg_database WHERE datname = @name";
        var parameters = new DynamicParameters();
        parameters.Add("name", databaseName);
        using IDbConnection connection = dataContextFactory.CreateMasterConnection();

        dynamic[] records = connection.Query(query, parameters).ToArray();

        if (records.Any() && isForTests)
        {
            connection.Execute($"DROP DATABASE {databaseName}");
        }

        if (!records.Any() || isForTests)
        {
            connection.Execute($"CREATE DATABASE {databaseName}");
        }
    }
}
