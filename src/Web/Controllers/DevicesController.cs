using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

using System.Collections.Concurrent;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly ILogger<DevicesController> logger;
    private static readonly ConcurrentDictionary<int, Device> Devices = new();

    public DevicesController(ILogger<DevicesController> logger)
    {
        this.logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Device[]), StatusCodes.Status200OK)] 
    public IActionResult GetDevices()
    {
        logger.LogInformation("Get statistics for devices");
        
        return Ok(Devices.Values);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(Device), StatusCodes.Status201Created)] 
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UploadDevice([FromBody] Device device)
    {
        if (Devices.TryGetValue(device.Id, out var existingDevice))
        {
            Devices.TryUpdate(device.Id, device, existingDevice);
            logger.LogInformation($"Upload statistics for device: {device.Id}");
            return Ok(device);
        }

        Devices.TryAdd(device.Id, device);
        logger.LogInformation($"Upload statistics for device: {device.Id}");
        return new ObjectResult(device) { StatusCode = StatusCodes.Status201Created };


    }
}
