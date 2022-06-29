using Infrastructure.Enums;
using Infrastructure.Interfaces;

namespace Infrastructure.Entities;

/// <summary>
/// Device
/// </summary>
public class Device : BaseEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Device"/> class.
    /// </summary>
    public Device() => LastUpdate = DateTime.UtcNow;

    /// <summary>
    /// FullName
    /// </summary>
    public string FullName { get; set; } = null!;

    /// <summary>
    /// Platform
    /// </summary>
    public Platform Platform { get; set; }

    /// <summary>
    /// Application
    /// </summary>
    public string Version { get; set; } = null!;

    /// <summary>
    /// LastUpdate
    /// </summary>
    public DateTime LastUpdate { get; set; }
}
