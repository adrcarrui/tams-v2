namespace Tams.Api.Contracts.Devices;

public sealed class CreateDeviceRequest
{
    public required string Name { get; init; }

    public int AssetVariantId { get; init; }

    public string? Uid { get; init; }

    public string? Barcode { get; init; }

    public string? SerialNumber { get; init; }

    public string? Notes { get; init; }
}