using Infrastructure;

namespace Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Infrastructure.Entities;
using MapsterMapper;
using Web.Models;
using Web.Models.Dtos;

/// <summary>
/// Device Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly ILogger<DevicesController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesController"/> class.
    /// </summary>
    /// <param name="unitOfWork">unitOfWork.</param>
    /// <param name="mapper">DeviceMapper.</param>
    /// <param name="logger">Logger.</param>
    public DevicesController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<DevicesController> logger)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.logger = logger;
    }

    /// <summary>
    /// Get devices
    /// </summary>
    /// <returns>IActionResult.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(Device[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDevices()
    {
        logger.LogInformation("Get statistics for devices");

        IReadOnlyCollection<Device> devices = await unitOfWork.DeviceRepository.GetAll();
        DeviceDto[] devicesDto = mapper.Map<DeviceDto[]>(devices);

        unitOfWork.SaveChanges();

        return Ok(devicesDto);
    }

    /// <summary>
    /// Upload devices
    /// </summary>
    /// <param name="device">Device.</param>
    /// <returns>IActionResult.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Device), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadDevice([FromBody] Device device)
    {
        Device? existingDevice = await unitOfWork.DeviceRepository.Get(device.Id);
        if (existingDevice != null)
        {
            await unitOfWork.DeviceRepository.Update(device);
            logger.LogInformation($"Upload statistics for device: {device.Id}");
            return Ok(device);
        }

        await unitOfWork.DeviceRepository.Insert(device);

        unitOfWork.SaveChanges();
        logger.LogInformation($"Upload statistics for device: {device.Id}");
        return new ObjectResult(device) { StatusCode = StatusCodes.Status201Created };
    }

    /// <summary>
    /// GetDeviceEvents
    /// </summary>
    /// <param name="deviceId">deviceId.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    [HttpGet("{deviceId:int}/events")]
    [ProducesResponseType(typeof(Event[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDeviceEvents([FromRoute] int deviceId)
    {
        logger.LogInformation("Get device's events");

        IReadOnlyCollection<Event> events = await unitOfWork.EventRepository.GetAllByDeviceId(deviceId);
        unitOfWork.SaveChanges();

        IEnumerable<EventDto> eventsDto = events.Select(x => mapper.Map<EventDto>(x));

        return Ok(new DeviceEventsDto { Id = deviceId, Events = eventsDto.ToList() });
    }

    /// <summary>
    /// Upload device events
    /// </summary>
    /// <param name="deviceId">deviceId.</param>
    /// <param name="eventInputs">events.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    [HttpPut("{deviceId:int}/events")]
    [ProducesResponseType(typeof(Device[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadDeviceEvents([FromRoute] int deviceId, [FromBody] IEnumerable<EventInput> eventInputs)
    {
        Device? existingDevice = await unitOfWork.DeviceRepository.Get(deviceId);
        if (existingDevice == null)
        {
            return BadRequest($"There is no device with id:{deviceId}");
        }

        Event[] events = mapper.Map<Event[]>(eventInputs);

        await unitOfWork.EventRepository.InsertBulk(events);

        unitOfWork.SaveChanges();

        EventDto[] eventsDto = mapper.Map<EventDto[]>(events);

        logger.LogInformation($"Upload device's events for device: {deviceId}");

        return new ObjectResult(eventsDto) { StatusCode = StatusCodes.Status201Created };
    }
}
