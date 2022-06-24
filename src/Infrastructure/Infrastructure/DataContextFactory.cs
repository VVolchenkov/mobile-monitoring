using System.Data;
using Npgsql;

namespace Infrastructure;

public class DataContextFactory
{
    private readonly string connectionString;

    public DataContextFactory(string connectionString)
    {
        this.connectionString = connectionString;
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }

}
