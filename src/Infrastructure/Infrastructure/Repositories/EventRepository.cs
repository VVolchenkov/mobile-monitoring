using System.Data;
using Dapper;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Z.BulkOperations;

namespace Infrastructure.Repositories;

public class EventRepository : GenericRepository<Event>, IEventRepository
{
    private readonly DataContextFactory contextFactory;

    public EventRepository(DataContextFactory contextFactory)
        : base(contextFactory) => this.contextFactory = contextFactory;

    public override Task Insert(Event eventEntity)
    {
        string query = @"INSERT INTO events(name, description, device_id, date) " +
            "VALUES (@Name, @Description, @DeviceId, @Date)";

        IDbConnection connection = contextFactory.CreateConnection();
        return connection.ExecuteAsync(query, eventEntity);
    }

    public override Task InsertBulk(IEnumerable<Event> eventEntities)
    {
        IDbConnection connection = contextFactory.CreateConnection();
        BulkOperation<Event> bulkOperation = connection.GetBulkOperation<Event>("events");
        bulkOperation.IgnoreOnInsertExpression = c => new { c.Id, c.Date };
        return bulkOperation.BulkInsertAsync(eventEntities);
    }

    public async Task<IReadOnlyCollection<Event>> GetAllByDeviceId(int deviceId)
    {
        var query = "SELECT d.id, e.date, e.name, e.description, e.device_id FROM devices d JOIN events e ON e.device_id = d.id WHERE d.id=@deviceId";

        IDbConnection connection = contextFactory.CreateConnection();
        IEnumerable<Event> events = await connection.QueryAsync<Event, Device, Event>(
            query,
            (eventEntity, device) =>
            {
                eventEntity.Device = device;
                return eventEntity;
            },
            splitOn: "device_id",
            param: new { deviceId });

        return events.ToList().AsReadOnly();
    }
}
