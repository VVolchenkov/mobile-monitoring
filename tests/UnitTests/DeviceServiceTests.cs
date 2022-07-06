using Infrastructure;
using Infrastructure.Entities;
using Infrastructure.RabbitMQ;
using MapsterMapper;
using Moq;
using Web.Controllers;
using Web.Models;
using Web.Services;
using Xunit;

namespace UnitTests;

public class DeviceServiceTests
{
    private readonly Mock<IUnitOfWork> mockUnitOfWork = new();
    private readonly Mock<IRabbitMqService> mockRabbitMqService = new();
    private readonly Mock<IMapper> mockMapper = new();
    private readonly IDeviceService deviceService;

    public DeviceServiceTests()
        => deviceService = new DeviceService(mockUnitOfWork.Object, mockRabbitMqService.Object, mockMapper.Object);

    [Fact]
    public void ShouldCallSuccessQueue()
    {
        // Arrange
        var deviceId = 1;
        var device = new Device { Id = deviceId };
        var eventInput = new EventInput
        {
            DeviceId = deviceId,
            Date = DateTime.UtcNow,
            Description = "description",
            Name = "name",
        };
        EventInput[] eventInputs = { eventInput };
        Event[] events = { new() { Id = 1, Name = eventInput.Name, Date = eventInput.Date, Description = eventInput.Description, DeviceId = eventInput.DeviceId } };
        EventDto[] eventsDto = { new() { Date = eventInput.Date, Description = eventInput.Description, Name = eventInput.Name} };

        mockMapper.Setup(x => x.Map<Event[]>(eventInputs)).Returns(events);
        mockMapper.Setup(x => x.Map<EventDto[]>(events)).Returns(eventsDto);

        mockUnitOfWork.Setup(x => x.DeviceRepository.Get(deviceId)).Returns(Task.FromResult(device)!);
        mockUnitOfWork.Setup(x => x.EventRepository.InsertBulk(events)).Returns(Task.CompletedTask);

        // Act
        deviceService.UploadDeviceEvents(deviceId, eventInputs);

        // Assert
        mockRabbitMqService.Verify(x => x.SendMessage(eventsDto[0], "EventsSuccess"), Times.Once);
    }

    [Fact]
    public void ShouldCallFailedQueue()
    {
        // Arrange
        var deviceId = 1;
        var device = new Device { Id = deviceId };
        var eventInput = new EventInput
        {
            DeviceId = deviceId,
            Date = DateTime.UtcNow,
            Description = "description",
            Name = "New Event for rabbitmq with name greater than 50 symbols",
        };
        EventInput[] eventInputs = { eventInput };
        Event[] events = { new() { Id = 1, Name = eventInput.Name, Date = eventInput.Date, Description = eventInput.Description, DeviceId = eventInput.DeviceId } };
        EventDto[] eventsDto = { new() { Date = eventInput.Date, Description = eventInput.Description, Name = eventInput.Name} };

        mockMapper.Setup(x => x.Map<Event[]>(eventInputs)).Returns(events);
        mockMapper.Setup(x => x.Map<EventDto[]>(events)).Returns(eventsDto);

        mockUnitOfWork.Setup(x => x.DeviceRepository.Get(deviceId)).Returns(Task.FromResult(device)!);
        mockUnitOfWork.Setup(x => x.EventRepository.InsertBulk(events)).Returns(Task.CompletedTask);

        // Act
        deviceService.UploadDeviceEvents(deviceId, eventInputs);

        // Assert
        mockRabbitMqService.Verify(x => x.SendMessage(eventsDto[0], "EventsFailed"), Times.Once);
    }
}
