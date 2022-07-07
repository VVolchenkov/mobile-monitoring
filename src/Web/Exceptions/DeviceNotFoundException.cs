namespace Web.Exceptions;

/// <inheritdoc />
internal class DeviceNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceNotFoundException"/> class.
    /// </summary>
    /// <param name="message">message.</param>
    public DeviceNotFoundException(string message)
        : base(message)
    {
    }
}
