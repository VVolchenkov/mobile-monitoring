using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class EventInput
{
    public int Id { get; set; }

    public EventInput() => Date = DateTime.UtcNow;

    public int DeviceId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }
}
