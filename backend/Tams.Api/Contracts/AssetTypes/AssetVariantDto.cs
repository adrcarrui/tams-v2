namespace Tams.Api.Contracts.AssetTypes;

public sealed class AssetVariantDto
{
    public int Id { get; init; }

    public required string Code { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public bool IsActive { get; init; }

    public int SortOrder { get; init; }
}