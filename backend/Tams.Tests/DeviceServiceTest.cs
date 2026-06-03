using Tams.Api.Application.Devices;
using Tams.Api.Contracts.Devices;
using Tams.Api.Domain.Entities;
using Tams.Api.Domain.Enums;
using Xunit;

namespace Tams.Tests;

public sealed class DeviceServiceTests
{
    [Fact]
    public async Task CreateDeviceAsync_WhenCardHasNoUid_ReturnsValidationError()
    {
        await using var dbContext = TestDbContextFactory.CreateDbContext();
        var service = new DeviceService(dbContext);

        var request = new CreateDeviceRequest
        {
            Name = "Card without UID",
            AssetVariantId = 1
        };

        var result = await service.CreateDeviceAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("DEVICE_UID_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task CreateDeviceAsync_WhenLaptopHasNoBarcode_ReturnsValidationError()
    {
        await using var dbContext = TestDbContextFactory.CreateDbContext();
        var service = new DeviceService(dbContext);

        var request = new CreateDeviceRequest
        {
            Name = "Laptop without barcode",
            AssetVariantId = 6
        };

        var result = await service.CreateDeviceAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("DEVICE_BARCODE_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task CreateDeviceAsync_WhenUidAlreadyExists_ReturnsValidationError()
    {
        await using var dbContext = TestDbContextFactory.CreateDbContext();

        dbContext.Devices.Add(new Device
        {
            Name = "Existing card",
            AssetVariantId = 1,
            Uid = "04AABBCC01",
            Status = DeviceStatus.Available,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        var service = new DeviceService(dbContext);

        var request = new CreateDeviceRequest
        {
            Name = "Duplicated card",
            AssetVariantId = 1,
            Uid = "04AABBCC01"
        };

        var result = await service.CreateDeviceAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("DEVICE_UID_ALREADY_EXISTS", result.ErrorCode);
    }

    [Fact]
    public async Task CreateDeviceAsync_WhenCardHasUid_CreatesAvailableDevice()
    {
        await using var dbContext = TestDbContextFactory.CreateDbContext();
        var service = new DeviceService(dbContext);

        var request = new CreateDeviceRequest
        {
            Name = "Card Vending 001",
            AssetVariantId = 1,
            Uid = "04AABBCC01",
            Notes = "Test card"
        };

        var result = await service.CreateDeviceAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Card Vending 001", result.Value!.Name);
        Assert.Equal("04AABBCC01", result.Value.Uid);
        Assert.Equal("Available", result.Value.Status);
    }

    [Fact]
    public async Task UpdateDeviceAsync_WhenKeepingSameUid_DoesNotReturnDuplicateError()
    {
        await using var dbContext = TestDbContextFactory.CreateDbContext();

        var device = new Device
        {
            Name = "Card Vending 001",
            AssetVariantId = 1,
            Uid = "04AABBCC01",
            Status = DeviceStatus.Available,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var service = new DeviceService(dbContext);

        var request = new UpdateDeviceRequest
        {
            Name = "Card Vending 001 Updated",
            AssetVariantId = 1,
            Uid = "04AABBCC01",
            Barcode = null,
            SerialNumber = null,
            Notes = "Updated",
            IsActive = true
        };

        var result = await service.UpdateDeviceAsync(device.Id, request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Card Vending 001 Updated", result.Value!.Name);
        Assert.Equal("04AABBCC01", result.Value.Uid);
    }

    [Fact]
    public async Task MarkDeviceAsLostAsync_WhenAvailable_SetsStatusLost()
    {
        await using var dbContext = TestDbContextFactory.CreateDbContext();

        var device = new Device
        {
            Name = "Laptop G10 001",
            AssetVariantId = 6,
            Barcode = "LAP-G10-001",
            Status = DeviceStatus.Available,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var service = new DeviceService(dbContext);

        var result = await service.MarkDeviceAsLostAsync(device.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Lost", result.Value!.Status);
        Assert.True(result.Value.IsActive);
    }

    [Fact]
    public async Task AnnulDeviceAsync_WhenAvailable_SetsStatusAnnulledAndInactive()
    {
        await using var dbContext = TestDbContextFactory.CreateDbContext();

        var device = new Device
        {
            Name = "USB Standard 001",
            AssetVariantId = 8,
            Barcode = "USB-001",
            Status = DeviceStatus.Available,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var service = new DeviceService(dbContext);

        var result = await service.AnnulDeviceAsync(device.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Annulled", result.Value!.Status);
        Assert.False(result.Value.IsActive);
    }

    [Fact]
    public async Task RestoreDeviceAsync_WhenLost_SetsStatusAvailableAndActive()
    {
        await using var dbContext = TestDbContextFactory.CreateDbContext();

        var device = new Device
        {
            Name = "Laptop G10 001",
            AssetVariantId = 6,
            Barcode = "LAP-G10-001",
            Status = DeviceStatus.Lost,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var service = new DeviceService(dbContext);

        var result = await service.RestoreDeviceAsync(device.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Available", result.Value!.Status);
        Assert.True(result.Value.IsActive);
    }

    [Fact]
    public async Task RestoreDeviceAsync_WhenAvailable_ReturnsValidationError()
    {
        await using var dbContext = TestDbContextFactory.CreateDbContext();

        var device = new Device
        {
            Name = "Laptop G10 001",
            AssetVariantId = 6,
            Barcode = "LAP-G10-001",
            Status = DeviceStatus.Available,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var service = new DeviceService(dbContext);

        var result = await service.RestoreDeviceAsync(device.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal("DEVICE_CANNOT_BE_RESTORED", result.ErrorCode);
    }
}