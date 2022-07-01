using Infrastructure;
using Infrastructure.Migrations.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests;

public class DatabaseFixture
{
    public IUnitOfWork UnitOfWork;

    public DatabaseFixture()
    {
        IConfigurationRoot? configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();
        var services = new ServiceCollection();

        services.AddInfrastructure(configuration);

        ServiceProvider? serviceProvider = services.BuildServiceProvider();

        var migrationManager = serviceProvider.GetRequiredService<MigrationManager>();
        migrationManager.MigrateDatabase(serviceProvider, configuration, true);

        UnitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
    }

    [CollectionDefinition("Database")]
    public class DatabaseCollectionFixture : ICollectionFixture<DatabaseFixture>
    {
    }
}
