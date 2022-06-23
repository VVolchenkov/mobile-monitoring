using Web.Enums;

namespace Web.Models;

public class Device
{
    public Device()
    {
        LastUpdate = DateTime.UtcNow;
    }
    
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public Platform Platform { get; set; }

    public Application Application { get; set; } = null!;

    public DateTime LastUpdate { get; set; }
}
