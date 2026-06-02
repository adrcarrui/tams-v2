using Tams.Api.Domain.Enums;

namespace Tams.Api.Domain.Entities;

public sealed class AssetType
{
    public int Id { get; set; }

    public required string Code { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public int ManagedByDepartmentId { get; set; }

    public Department? ManagedByDepartment { get; set; }

    public AssetIdentifierPolicy IdentifierPolicy { get; set; }

    public bool CanBeAssignedToCourse { get; set; }

    public bool IsReturnable { get; set; }

    public bool ShowInCalendar { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public string? Icon { get; set; }

    public string? Color { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public List<AssetVariant> Variants { get; set; } = [];
}