using Web.Models;

namespace Web.Controllers;

public interface IDeviceService
{
    Task<IReadOnlyCollection<EventDto>> UploadDeviceEvents(int deviceId, EventInput[] eventInputs);

    Task DeleteDeviceEvents(int deviceId);

    Task UpdateDeviceEvent(int deviceId, EventInput eventInput);
}
