namespace Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Mapster;
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
    private readonly IMapper mapper;
    private readonly IDeviceRepository deviceRepository;
    private readonly IEventRepository eventRepository;
    private readonly ILogger<DevicesController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesController"/> class.
    /// </summary>
    /// <param name="deviceRepository">DeviceRepository.</param>
    /// <param name="eventRepository">EventRepository.</param>
    /// <param name="mapper">DeviceMapper.</param>
    /// <param name="logger">Logger.</param>
    public DevicesController(
        IDeviceRepository deviceRepository,
        IEventRepository eventRepository,
        IMapper mapper,
        ILogger<DevicesController> logger)
    {
        this.deviceRepository = deviceRepository;
        this.eventRepository = eventRepository;
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

        var devices = await deviceRepository.GetAll();
        var devicesDto = devices.Select(x => mapper.Map<DeviceDto>(x));

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
        var existingDevice = await deviceRepository.Get(device.Id);
        if (existingDevice != null)
        {
            await deviceRepository.Update(device);
            logger.LogInformation($"Upload statistics for device: {device.Id}");
            return Ok(device);
        }

        await deviceRepository.Insert(device);
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

        var events = await eventRepository.GetAllByDeviceId(deviceId);
        var eventsDto = events.Select(x => mapper.Map<EventDto>(x));

        return Ok(new DeviceEventsDto { Id = deviceId, Events = eventsDto.ToList() });
    }

    /// <summary>
    /// Upload device events
    /// </summary>
    /// <param name="deviceId">deviceId.</param>
    /// <param name="events">events.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    [HttpPut("{deviceId:int}/events")]
    [ProducesResponseType(typeof(Device[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadDeviceEvents([FromRoute] int deviceId, [FromBody] IEnumerable<Event> events)
    {
        var existingDevice = await deviceRepository.Get(deviceId);
        if (existingDevice == null)
        {
            return BadRequest($"There is no device with id:{deviceId}");
        }

        await eventRepository.InsertBulk(events);

        logger.LogInformation($"Upload device's events for device: {deviceId}");

        return new ObjectResult(events) { StatusCode = StatusCodes.Status201Created };
    }
}