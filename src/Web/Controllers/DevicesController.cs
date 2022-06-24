namespace Web.Controllers;

using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Entities;

/// <summary>
/// Device Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IRepository<Device> deviceRepository;
    private readonly ILogger<DevicesController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesController"/> class.
    /// </summary>
    /// <param name="deviceRepository">DeviceRepository.</param>
    /// <param name="logger">Logger.</param>
    public DevicesController(IRepository<Device> deviceRepository, ILogger<DevicesController> logger)
    {
        this.deviceRepository = deviceRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Get devices
    /// </summary>
    /// <returns>IActionResult.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(Device[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDevices()
    {
        logger.LogInformation("Get statistics for devices");

        var devices = await deviceRepository.GetAll();

        return Ok(devices);
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
}