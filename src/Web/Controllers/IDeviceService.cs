using Web.Models;

namespace Web.Controllers;

public interface IDeviceService
{
    Task<IReadOnlyCollection<EventDto>> UploadDeviceEvents(int deviceId, EventInput[] eventInputs);
}
