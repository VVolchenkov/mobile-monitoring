using System.Data;
using Dapper;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class DeviceRepository : GenericRepository<Device>, IDeviceRepository
{
    private readonly DataContextFactory contextFactory;

    public DeviceRepository(DataContextFactory contextFactory)
        : base(contextFactory) => this.contextFactory = contextFactory;

    public override Task Update(Device device)
    {
        var query = "UPDATE devices SET id=@Id, full_name=@FullName, " +
            "platform=@platform, version=@Version, last_update=@LastUpdate WHERE id=@Id";

        IDbConnection connection = contextFactory.CreateConnection();
        return connection.ExecuteAsync(query, device);
    }

    public override Task Insert(Device device)
    {
        string query = @"INSERT INTO devices(id, full_name, platform, version, last_update) " +
            "VALUES (@Id, @FullName, @Platform, @Version, @LastUpdate)";

        IDbConnection connection = contextFactory.CreateConnection();
        return connection.ExecuteAsync(query, device);
    }
}
