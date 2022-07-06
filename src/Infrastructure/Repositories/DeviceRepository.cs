using Dapper;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class DeviceRepository : GenericRepository<Device>, IDeviceRepository
{
    public DeviceRepository(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public override Task Update(Device device)
    {
        string query = "UPDATE devices SET id=@Id, full_name=@FullName, " +
            "platform=@platform, version=@Version, last_update=@LastUpdate WHERE id=@Id";

        return UnitOfWork.Connection.ExecuteAsync(query, device, UnitOfWork.Transaction);
    }

    public override Task Insert(Device device)
    {
        string query = @"INSERT INTO devices(id, full_name, platform, version, last_update) " +
            "VALUES (@Id, @FullName, @Platform, @Version, @LastUpdate)";

        return UnitOfWork.Connection.ExecuteAsync(query, device, UnitOfWork.Transaction);
    }
}
