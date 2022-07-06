using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Migrations.Extensions;

public class MigrationManager
{
    public void MigrateDatabase(IServiceProvider serviceProvider, IConfigurationRoot configuration, bool isForTests = false)
    {
        if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        using IServiceScope scope = serviceProvider.CreateScope();
        var databaseService = scope.ServiceProvider.GetRequiredService<Database>();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        string? connectionString = configuration.GetConnectionString("PostgreSql");
        var connection = new NpgsqlConnection(connectionString);

        try
        {
            databaseService.CreateDatabase(connection.Database, isForTests);

            migrationService.ListMigrations();
            migrationService.MigrateUp();
        }
        catch (Exception e)
        {
            throw new Exception("Migration error", e);
        }
    }
}
