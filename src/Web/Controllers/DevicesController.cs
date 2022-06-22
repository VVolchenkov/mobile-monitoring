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
    public IActionResult GetDevices()
    {
        logger.LogInformation("Get statistics for devices");
        
        if (Devices.Count <= 0)
        {
            NotFound("There are no devices yet");
        }
        
        return Ok(Devices.Values);
    }
    
    [HttpPost]
    public IActionResult UploadDevice([FromBody] Device device)
    {
        if (Devices.TryGetValue(device.Id, out _))
        {
            return BadRequest($"Device with id: {device.Id} already exists. Use Put method instead");
        }

        Devices.TryAdd(device.Id, device);
        logger.LogInformation($"Upload statistics for device: {device.Id}");

        return new ObjectResult(device) { StatusCode = StatusCodes.Status201Created };
    }
    
    [HttpPut("{deviceId:int}")]
    public IActionResult UpdateDevice([FromRoute] int deviceId, [FromBody] Device device)
    {
        if (!Devices.TryGetValue(deviceId, out var existingDevice))
        {
            return BadRequest($"Device with id: {deviceId} doesn't exist. Use Post method instead");
        }

        Devices.TryUpdate(deviceId, device, existingDevice);
        logger.LogInformation($"Upload statistics for device: {device.Id}");

        return Ok(device);
    }
}
