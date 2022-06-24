using Dapper;
using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public class DeviceRepository : IRepository<Device>
{
    private readonly DataContextFactory contextFactory;

    public DeviceRepository(DataContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async Task<Device?> Get(int id)
    {
        var query = "SELECT * FROM devices WHERE id=@id";

        var connection = contextFactory.CreateConnection();
        var device = await connection.QueryFirstOrDefaultAsync<Device>(query, new { id });

        return device;
    }

    public async Task<ICollection<Device>> GetAll()
    {
        var query = "SELECT * FROM devices";

        var connection = contextFactory.CreateConnection();
        var devices = await connection.QueryAsync<Device>(query);

        return devices.ToList();
    }

    public async Task Update(Device device)
    {
        var query = "UPDATE devices SET id=@Id, full_name=@FullName, platform=@platform, version=@Version, last_update=@LastUpdate";

        var connection = contextFactory.CreateConnection();
        await connection.ExecuteAsync(query, device);
    }

    public async Task Insert(Device device)
    {
        var query = @"INSERT INTO devices(id, full_name, platform, version, last_update) " +
            "VALUES (@Id, @FullName, @Platform, @Version, @LastUpdate)";

        var connection = contextFactory.CreateConnection();
        await connection.ExecuteAsync(query, device);
    }
}
