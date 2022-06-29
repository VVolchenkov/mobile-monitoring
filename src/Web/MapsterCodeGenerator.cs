namespace Web;

using Infrastructure.Entities;
using Mapster;
using Web.Models;

/// <inheritdoc />
public class MapsterCodeGenerator : ICodeGenerationRegister
{
    /// <inheritdoc/>
    public void Register(CodeGenerationConfig config) =>
        config
            .AdaptTo("[name]Dto")
            .ForType<Device>(cfg =>
            {
                cfg.Map(x => x.Platform, "Os");
                cfg.Map(x => x.FullName, "Name");
            })
            .ForType<Event>(cfg =>
            {
                cfg.Ignore(x => x.Id);
                cfg.Ignore(x => x.DeviceId);
                cfg.Ignore(x => x.Device);
            });
    // Не удалось найти возможность кастомизировать генерируемые Mapper'ы
    // config.GenerateMapper("[name]Mapper")
    //     .ForType<Device>()
    //     .ForType<Event>();
}