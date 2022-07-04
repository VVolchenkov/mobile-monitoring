using Infrastructure.Interfaces;

namespace Infrastructure.Entities;

public class Event : BaseEntity
{
    public Event() => Date = DateTime.UtcNow;

    public int DeviceId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }

    public Device Device { get; set; } = null!;
}
