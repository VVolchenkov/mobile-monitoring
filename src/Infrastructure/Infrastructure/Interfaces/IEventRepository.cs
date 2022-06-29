using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<IReadOnlyCollection<Event>> GetAllByDeviceId(int deviceId);
}
