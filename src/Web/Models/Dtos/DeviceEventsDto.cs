namespace Web.Models.Dtos;

/// <summary>
/// DeviceEvents
/// </summary>
public class DeviceEventsDto
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Events
    /// </summary>
    public ICollection<EventDto> Events { get; set; } = null!;
}
