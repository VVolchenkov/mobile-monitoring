using System.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Infrastructure;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    public IDbTransaction Transaction { get; }
    public IDbConnection Connection { get; }
    public IDeviceRepository DeviceRepository => new DeviceRepository(this);
    public IEventRepository EventRepository => new EventRepository(this);

    public UnitOfWork(DataContextFactory dataContextFactory)
    {
        Connection = dataContextFactory.CreateConnection();
        Transaction = Connection.BeginTransaction();
    }

    public void SaveChanges()
    {
        try
        {
            Transaction.Commit();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Can not commit transaction. {e}");
            Transaction.Rollback();
        }
    }

    public void Rollback()
    {
        try
        {
            Transaction.Rollback();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Can not rollback transaction. {e}");
        }
    }

    public void Dispose()
    {
        Connection.Close();
        Transaction.Dispose();
        Connection.Dispose();
    }
}
