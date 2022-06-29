using FluentAssertions;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Xunit;

namespace IntegrationTests.RepositoryTests;

[Collection("Database")]
public class DeviceRepositoryTests
{
    private static int nextId;
    private int id;
    private readonly IDeviceRepository deviceRepository;

    public DeviceRepositoryTests(DatabaseFixture databaseFixture)
        => deviceRepository = new DeviceRepository(databaseFixture.DataContextFactory);

    private int GetNextId()
    {
        id = Interlocked.Increment(ref nextId);
        return id;
    }

    [Fact]
    public async Task ShouldInsertAndGetDevice()
    {
        // Arrange
        var arrangeDevice = new Device
        {
            Id = GetNextId(),
            Platform = Platform.Android,
            Version = "1.0",
            FullName = "FullName",
            LastUpdate = DateTime.UtcNow,
        };

        // Act
        await deviceRepository.Insert(arrangeDevice);

        Device? device = await deviceRepository.Get(arrangeDevice.Id);

        // Assert
        device.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldInsertAndUpdateDevice()
    {
        // Arrange
        var arrangeDevice = new Device
        {
            Id = GetNextId(),
            Platform = Platform.Android,
            Version = "1.0",
            FullName = "FullName",
            LastUpdate = DateTime.UtcNow,
        };

        var changedPlatform = Platform.Windows;

        // Act
        await deviceRepository.Insert(arrangeDevice);

        Device? deviceAfterInsert = await deviceRepository.Get(arrangeDevice.Id);

        deviceAfterInsert.Should().NotBeNull();
        deviceAfterInsert!.Platform.Should().Be(arrangeDevice.Platform);

        arrangeDevice.Platform = changedPlatform;
        await deviceRepository.Update(arrangeDevice);

        Device? deviceAfterUpdate = await deviceRepository.Get(arrangeDevice.Id);

        // Assert
        deviceAfterUpdate.Should().NotBeNull();
        deviceAfterUpdate!.Platform.Should().Be(changedPlatform);
    }

    [Fact]
    public async Task ShouldInsertGetAllDevices()
    {
        // Arrange
        var arrangeDevice1 = new Device
        {
            Id = GetNextId(),
            Platform = Platform.Android,
            Version = "1.0",
            FullName = "FullName",
            LastUpdate = DateTime.UtcNow,
        };

        var arrangeDevice2 = new Device
        {
            Id = GetNextId(),
            Platform = Platform.Android,
            Version = "1.0",
            FullName = "FullName",
            LastUpdate = DateTime.UtcNow,
        };

        // Act
        await deviceRepository.Insert(arrangeDevice1);
        await deviceRepository.Insert(arrangeDevice2);

        IReadOnlyCollection<Device> devices = await deviceRepository.GetAll();

        // Assert
        devices.Should().NotBeNull();
        devices.Count.Should().Be(GetNextId() - 1);
    }
}
