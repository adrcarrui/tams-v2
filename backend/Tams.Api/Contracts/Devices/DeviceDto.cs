namespace Tams.Api.Contracts.Devices;

public sealed class DeviceDto
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public int AssetVariantId { get; init; }

    public required string AssetVariantCode { get; init; }

    public required string AssetVariantName { get; init; }

    public int AssetTypeId { get; init; }

    public required string AssetTypeCode { get; init; }

    public required string AssetTypeName { get; init; }

    public int ManagedByDepartmentId { get; init; }

    public required string ManagedByDepartmentCode { get; init; }

    public string? Uid { get; init; }

    public string? Barcode { get; init; }

    public string? SerialNumber { get; init; }

    public required string Status { get; init; }

    public bool IsActive { get; init; }

    public string? Notes { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }
}