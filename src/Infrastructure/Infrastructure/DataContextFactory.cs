using System.Data;
using Npgsql;

namespace Infrastructure;

public class DataContextFactory
{
    private readonly string connectionString;
    private readonly string masterConnectionString;

    public DataContextFactory(string connectionString, string masterConnectionString)
    {
        this.connectionString = connectionString;
        this.masterConnectionString = masterConnectionString;
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }

    public IDbConnection CreateMasterConnection()
    {
        var connection = new NpgsqlConnection(masterConnectionString);
        connection.Open();
        return connection;
    }
}
