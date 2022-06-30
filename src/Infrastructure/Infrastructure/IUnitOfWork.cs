using System.Data;
using Infrastructure.Interfaces;

namespace Infrastructure;

public interface IUnitOfWork
{
    IDeviceRepository DeviceRepository { get; }
    IEventRepository EventRepository { get; }
    IDbTransaction Transaction { get; }
    IDbConnection Connection { get; }
    void SaveChanges();
    void Rollback();
}
