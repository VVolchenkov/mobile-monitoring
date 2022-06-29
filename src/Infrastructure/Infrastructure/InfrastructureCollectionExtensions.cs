using System.Reflection;
using FluentMigrator.Runner;
using Infrastructure.Interfaces;
using Infrastructure.Migrations;
using Infrastructure.Migrations.Extensions;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfigurationRoot configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        string connectionString = configuration.GetConnectionString("PostgreSql");
        services.AddSingleton(new DataContextFactory(
            connectionString,
            configuration.GetConnectionString("MasterConnection")));
        services.AddSingleton<Database>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IEventRepository, EventRepository>();

        AddFluentMigrator(services, connectionString);
    }

    private static void AddFluentMigrator(IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new MigrationManager());
        Assembly infrastructureAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .Single(assembly => assembly.GetName().Name == "Infrastructure");
        services.AddFluentMigratorCore()
            .ConfigureRunner(rb =>
            {
                rb.AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(infrastructureAssembly).For.Migrations();
            });
    }
}
