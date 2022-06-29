using System.Reflection;
using FluentMigrator.Runner;
using Infrastructure;
using Infrastructure.Migrations;
using Infrastructure.Migrations.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;

namespace IntegrationTests;

public class DatabaseFixture
{
    public DataContextFactory DataContextFactory;

    public DatabaseFixture()
    {
        IConfigurationRoot? configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();
        var services = new ServiceCollection();

        services.AddInfrastructure(configuration);

        ServiceProvider? serviceProvider = services.BuildServiceProvider();
        DataContextFactory = serviceProvider.GetRequiredService<DataContextFactory>();

        var migrationManager = serviceProvider.GetRequiredService<MigrationManager>();
        migrationManager.MigrateDatabase(serviceProvider, configuration, true);
    }

    [CollectionDefinition("Database")]
    public class DatabaseCollectionFixture : ICollectionFixture<DatabaseFixture>
    {
    }
}
