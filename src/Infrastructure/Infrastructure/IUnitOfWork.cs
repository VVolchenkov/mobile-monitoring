using System.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Infrastructure;

public interface IUnitOfWork
{
    IDeviceRepository DeviceRepository { get; }
    IEventRepository EventRepository { get; }
    IDbTransaction Transaction { get; }
    IDbConnection Connection { get; }
    void SaveChanges();
}
