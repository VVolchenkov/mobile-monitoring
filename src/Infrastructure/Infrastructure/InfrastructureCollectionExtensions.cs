using Infrastructure.Entities;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new DataContextFactory(connectionString));
        services.AddScoped<IRepository<Device>, DeviceRepository>();
    }
}
