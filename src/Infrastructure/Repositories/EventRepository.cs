using Dapper;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Z.BulkOperations;

namespace Infrastructure.Repositories;

public class EventRepository : GenericRepository<Event>, IEventRepository
{
    public EventRepository(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public override Task Insert(Event eventEntity)
    {
        string query = @"INSERT INTO events(name, description, device_id, date) " +
            "VALUES (@Name, @Description, @DeviceId, @Date)";

        return UnitOfWork.Connection.ExecuteAsync(query, eventEntity, UnitOfWork.Transaction);
    }

    public override Task InsertBulk(IEnumerable<Event> eventEntities)
    {
        BulkOperation<Event> bulkOperation = UnitOfWork.Connection.GetBulkOperation<Event>("events", UnitOfWork);
        bulkOperation.IgnoreOnInsertExpression = c => new { c.Id, c.Date };

        return bulkOperation.BulkInsertAsync(eventEntities);
    }

    public async Task<IReadOnlyCollection<Event>> GetAllByDeviceId(int deviceId)
    {
        string query = "SELECT e.id, d.id as deviceId, e.date, e.name, e.description, e.device_id " +
            "FROM devices d JOIN events e ON e.device_id = d.id WHERE d.id=@deviceId";

        IEnumerable<Event> events = await UnitOfWork.Connection.QueryAsync<Event, Device, Event>(
            query,
            (eventEntity, device) =>
            {
                eventEntity.Device = device;
                return eventEntity;
            },
            splitOn: "device_id",
            param: new { deviceId },
            transaction: UnitOfWork.Transaction);

        return events.ToList().AsReadOnly();
    }

    public Task<int> DeleteAllByDeviceId(int deviceId)
    {
        var query = "DELETE FROM events WHERE device_id=@deviceId";

        return UnitOfWork.Connection.ExecuteAsync(query, new { deviceId }, UnitOfWork.Transaction);
    }

    public override Task Update(Event eventEntity)
    {
        string query = "UPDATE events SET name=@Name, " +
        "date=@Date, device_id=@DeviceId, description=@Description WHERE id=@Id";

        return UnitOfWork.Connection.ExecuteAsync(query, eventEntity, UnitOfWork.Transaction);
    }
}
