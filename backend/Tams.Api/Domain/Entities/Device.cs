using Tams.Api.Domain.Enums;

namespace Tams.Api.Domain.Entities;

public sealed class Device
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int AssetVariantId { get; set; }

    public AssetVariant? AssetVariant { get; set; }

    public string? Uid { get; set; }

    public string? Barcode { get; set; }

    public string? SerialNumber { get; set; }

    public DeviceStatus Status { get; set; } = DeviceStatus.Available;

    public bool IsActive { get; set; } = true;

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}