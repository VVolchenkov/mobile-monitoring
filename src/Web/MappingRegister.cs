namespace Web;

using Infrastructure.Entities;
using Mapster;
using Web.Models;

/// <inheritdoc />
public class MappingRegister : IRegister
{
    /// <inheritdoc />
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<Device, DeviceDto>()
            .Map(d => d.Os, s => s.Platform)
            .Map(d => d.Name, s => s.FullName);

        config.NewConfig<Event, EventDto>();
        config.NewConfig<EventInput, Event>();
    }
}