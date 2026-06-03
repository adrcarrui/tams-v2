using System.Net;
using System.Net.Http.Json;
using Tams.Api.Contracts.Common;
using Tams.Api.Contracts.Devices;

namespace Tams.Tests;

public sealed class DevicesEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DevicesEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDevices_WhenNoDevicesExist_ReturnsEmptyPagedResult()
    {
        var result = await _client.GetFromJsonAsync<PagedResultDto<DeviceDto>>("/api/devices");

        Assert.NotNull(result);
        Assert.Empty(result!.Items);
        Assert.Equal(1, result.Page);
        Assert.Equal(25, result.PageSize);
        Assert.Equal(0, result.TotalItems);
    }

    [Fact]
    public async Task CreateDevice_WhenCardHasUid_ReturnsCreatedDevice()
    {
        var request = new CreateDeviceRequest
        {
            Name = "Card Vending 001",
            AssetVariantId = 1,
            Uid = "04AABBCC01",
            Notes = "Integration test card"
        };

        var response = await _client.PostAsJsonAsync("/api/devices", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var device = await response.Content.ReadFromJsonAsync<DeviceDto>();

        Assert.NotNull(device);
        Assert.Equal("Card Vending 001", device!.Name);
        Assert.Equal("04AABBCC01", device.Uid);
        Assert.Equal("Available", device.Status);
        Assert.Equal("VENDING", device.AssetVariantCode);
        Assert.Equal("CARD", device.AssetTypeCode);
    }

    [Fact]
    public async Task CreateDevice_WhenCardHasNoUid_ReturnsBadRequest()
    {
        var request = new CreateDeviceRequest
        {
            Name = "Bad Card",
            AssetVariantId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/devices", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorDto>();

        Assert.NotNull(error);
        Assert.Equal("DEVICE_UID_REQUIRED", error!.Code);
    }

    [Fact]
    public async Task GetDeviceById_WhenDeviceExists_ReturnsDevice()
    {
        var createRequest = new CreateDeviceRequest
        {
            Name = "Laptop G10 001",
            AssetVariantId = 6,
            Barcode = "LAP-G10-001",
            SerialNumber = "SN-G10-001"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/devices", createRequest);

        var createdDevice = await createResponse.Content.ReadFromJsonAsync<DeviceDto>();

        var response = await _client.GetAsync($"/api/devices/{createdDevice!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var device = await response.Content.ReadFromJsonAsync<DeviceDto>();

        Assert.NotNull(device);
        Assert.Equal(createdDevice.Id, device!.Id);
        Assert.Equal("Laptop G10 001", device.Name);
        Assert.Equal("LAPTOP", device.AssetTypeCode);
        Assert.Equal("G10", device.AssetVariantCode);
    }

    [Fact]
    public async Task MarkDeviceAsLost_WhenDeviceExists_ReturnsLostDevice()
    {
        var createRequest = new CreateDeviceRequest
        {
            Name = "USB Standard 001",
            AssetVariantId = 8,
            Barcode = "USB-001"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/devices", createRequest);

        var createdDevice = await createResponse.Content.ReadFromJsonAsync<DeviceDto>();

        var response = await _client.PatchAsync($"/api/devices/{createdDevice!.Id}/mark-lost", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var device = await response.Content.ReadFromJsonAsync<DeviceDto>();

        Assert.NotNull(device);
        Assert.Equal("Lost", device!.Status);
        Assert.True(device.IsActive);
    }
}