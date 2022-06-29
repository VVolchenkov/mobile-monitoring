using Dapper;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class DeviceRepository : GenericRepository<Device>, IDeviceRepository
{
    private readonly IUnitOfWork unitOfWork;

    public DeviceRepository(IUnitOfWork unitOfWork)
        : base(unitOfWork) => this.unitOfWork = unitOfWork;

    public override Task Update(Device device)
    {
        string query = "UPDATE devices SET id=@Id, full_name=@FullName, " +
            "platform=@platform, version=@Version, last_update=@LastUpdate WHERE id=@Id";

        return unitOfWork.Connection.ExecuteAsync(query, device, unitOfWork.Transaction);
    }

    public override Task Insert(Device device)
    {
        string query = @"INSERT INTO devices(id, full_name, platform, version, last_update) " +
            "VALUES (@Id, @FullName, @Platform, @Version, @LastUpdate)";

        return unitOfWork.Connection.ExecuteAsync(query, device, unitOfWork.Transaction);
    }
}
