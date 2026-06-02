namespace Tams.Api.Domain.Entities;

public sealed class AssetVariant
{
    public int Id { get; set; }

    public int AssetTypeId { get; set; }

    public AssetType? AssetType { get; set; }

    public required string Code { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}