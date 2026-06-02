namespace Tams.Api.Contracts.AssetTypes;

public sealed class AssetTypeDto
{
    public int Id { get; init; }

    public required string Code { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public int ManagedByDepartmentId { get; init; }

    public required string ManagedByDepartmentCode { get; init; }

    public required string IdentifierPolicy { get; init; }

    public bool CanBeAssignedToCourse { get; init; }

    public bool IsReturnable { get; init; }

    public bool ShowInCalendar { get; init; }

    public bool IsActive { get; init; }

    public int SortOrder { get; init; }

    public string? Icon { get; init; }

    public string? Color { get; init; }

    public IReadOnlyList<AssetVariantDto> Variants { get; init; } = [];
}