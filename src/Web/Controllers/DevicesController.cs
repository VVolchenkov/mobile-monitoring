namespace Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Web.Models;
using System.Collections.Concurrent;

/// <summary>
/// Device Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly ILogger<DevicesController> logger;
    private static readonly ConcurrentDictionary<int, Device> Devices = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesController"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public DevicesController(ILogger<DevicesController> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Get devices
    /// </summary>
    /// <returns>IActionResult.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(Device[]), StatusCodes.Status200OK)]
    public IActionResult GetDevices()
    {
        this.logger.LogInformation("Get statistics for devices");

        return this.Ok(Devices.Values);
    }

    /// <summary>
    /// Upload devices
    /// </summary>
    /// <param name="device">Device.</param>
    /// <returns>IActionResult.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Device), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UploadDevice([FromBody] Device device)
    {
        if (Devices.TryGetValue(device.Id, out var existingDevice))
        {
            Devices.TryUpdate(device.Id, device, existingDevice);
            this.logger.LogInformation($"Upload statistics for device: {device.Id}");
            return this.Ok(device);
        }

        Devices.TryAdd(device.Id, device);
        this.logger.LogInformation($"Upload statistics for device: {device.Id}");
        return new ObjectResult(device) { StatusCode = StatusCodes.Status201Created };
    }
}