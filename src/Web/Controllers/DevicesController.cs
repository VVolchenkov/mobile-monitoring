using Infrastructure;
using Infrastructure.RabbitMQ;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Entities;
using MapsterMapper;
using Web.Exceptions;
using Web.Models;
using Web.Models.Dtos;

namespace Web.Controllers;

/// <summary>
/// Device Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService deviceService;
    private readonly IUnitOfWork unitOfWork;
    private readonly IHubContext<DeviceHub> deviceHub;
    private readonly IMapper mapper;
    private readonly ILogger<DevicesController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesController"/> class.
    /// </summary>
    /// <param name="deviceService">deviceService.</param>
    /// <param name="unitOfWork">unitOfWork.</param>
    /// <param name="deviceHub">deviceHub.</param>
    /// <param name="mapper">DeviceMapper.</param>
    /// <param name="logger">Logger.</param>
    public DevicesController(
        IDeviceService deviceService,
        IUnitOfWork unitOfWork,
        IHubContext<DeviceHub> deviceHub,
        IMapper mapper,
        ILogger<DevicesController> logger)
    {
        this.deviceService = deviceService;
        this.unitOfWork = unitOfWork;
        this.deviceHub = deviceHub;
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
            unitOfWork.SaveChanges();
            logger.LogInformation($"Upload statistics for device: {device.Id}");
            return Ok(device);
        }

        await unitOfWork.DeviceRepository.Insert(device);

        unitOfWork.SaveChanges();

        var deviceDto = mapper.Map<DeviceDto>(device);
        await deviceHub.Clients.All.SendAsync("uploadDevice", deviceDto);

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
        try
        {
            logger.LogInformation($"Upload device's events for device: {deviceId}");

            IReadOnlyCollection<EventDto> eventsDto = await deviceService.UploadDeviceEvents(deviceId, eventInputs.ToArray());

            return new ObjectResult(eventsDto) { StatusCode = StatusCodes.Status201Created };
        }
        catch (DeviceNotFoundException e)
        {
            return BadRequest(e);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpPut("{deviceId:int}/events/{eventId:int}")]
    public async Task<IActionResult> UpdateDeviceEvent([FromRoute] int deviceId, [FromBody] EventInput eventInput)
    {
        try
        {
            await deviceService.UpdateDeviceEvent(deviceId, eventInput);
            return Ok();
        }
        catch (DeviceNotFoundException e)
        {
            return BadRequest(e);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpDelete("{deviceId:int}/events")]
    public async Task<IActionResult> DeleteDeviceEvent([FromRoute] int deviceId)
    {
        try
        {
            logger.LogInformation($"Delete device's events for device: {deviceId}");

            await deviceService.DeleteDeviceEvents(deviceId);

            return NoContent();
        }
        catch (DeviceNotFoundException e)
        {
            return BadRequest(e);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
}
