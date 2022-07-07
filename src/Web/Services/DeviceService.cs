using Infrastructure;
using Infrastructure.Entities;
using Infrastructure.RabbitMQ;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers;
using Web.Exceptions;
using Web.Models;

namespace Web.Services;

/// <inheritdoc />
public class DeviceService : IDeviceService
{
    private const int EventNameMaxLength = 50;
    private readonly IUnitOfWork unitOfWork;
    private readonly IRabbitMqService rabbitMqService;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceService"/> class.
    /// </summary>
    /// <param name="unitOfWork">unitOfWork.</param>
    /// <param name="rabbitMqService">rabbitMqService.</param>
    /// <param name="mapper">mapper.</param>
    public DeviceService(IUnitOfWork unitOfWork, IRabbitMqService rabbitMqService, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.rabbitMqService = rabbitMqService;
        this.mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<EventDto>> UploadDeviceEvents(int deviceId, EventInput[] eventInputs)
    {
        Device? existingDevice = await unitOfWork.DeviceRepository.Get(deviceId);
        if (existingDevice == null)
        {
            throw new DeviceNotFoundException($"There is no device with id:{deviceId}");
        }

        EventInput[] eventsInputArray = eventInputs.ToArray();
        EventInput[] validatedEvents = ValidateEvents(eventsInputArray);

        Event[] failedEvents = mapper.Map<Event[]>(eventsInputArray.Except(validatedEvents));
        Event[] events = mapper.Map<Event[]>(validatedEvents);
        await unitOfWork.EventRepository.InsertBulk(events);
        unitOfWork.SaveChanges();

        EventDto[] eventsDto = mapper.Map<EventDto[]>(events);
        EventDto[] failedEventsDto = mapper.Map<EventDto[]>(failedEvents);

        foreach (EventDto eventDto in eventsDto)
        {
            rabbitMqService.SendMessage(eventDto, "EventsSuccess");
        }

        foreach (EventDto eventDto in failedEventsDto)
        {
            rabbitMqService.SendMessage(eventDto, "EventsFailed");
        }

        return eventsDto.ToList().AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task DeleteDeviceEvents(int deviceId)
    {
        Device? existingDevice = await unitOfWork.DeviceRepository.Get(deviceId);
        if (existingDevice == null)
        {
            throw new DeviceNotFoundException($"There is no device with id:{deviceId}");
        }

        await unitOfWork.EventRepository.DeleteAllByDeviceId(deviceId);

        unitOfWork.SaveChanges();
    }

    public async Task UpdateDeviceEvent(int deviceId, EventInput eventInput)
    {
        Device? existingDevice = await unitOfWork.DeviceRepository.Get(deviceId);
        if (existingDevice == null)
        {
            throw new DeviceNotFoundException($"There is no device with id:{deviceId}");
        }

        var eventEntity = mapper.Map<Event>(eventInput);

        await unitOfWork.EventRepository.Update(eventEntity);

        unitOfWork.SaveChanges();
    }

    private EventInput[] ValidateEvents(IEnumerable<EventInput> eventInputs)
        => eventInputs.Where(x => x.Name.Length <= EventNameMaxLength).ToArray();
}
