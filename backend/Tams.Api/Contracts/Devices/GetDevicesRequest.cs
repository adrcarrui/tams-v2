namespace Tams.Api.Contracts.Devices;

public sealed class GetDevicesRequest
{
    public string? Search { get; init; }

    public string? Status { get; init; }

    public int? AssetTypeId { get; init; }

    public int? AssetVariantId { get; init; }

    public bool? IsActive { get; init; } = true;

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 25;
}