namespace Web.Models;

public class EventInput
{
    public EventInput() => Date = DateTime.UtcNow;

    public int DeviceId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }
}
