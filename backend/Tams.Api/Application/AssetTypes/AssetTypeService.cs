using Microsoft.EntityFrameworkCore;
using Tams.Api.Contracts.AssetTypes;
using Tams.Api.Infrastructure.Data;

namespace Tams.Api.Application.AssetTypes;

public sealed class AssetTypeService
{
    private readonly TamsDbContext _dbContext;

    public AssetTypeService(TamsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<AssetTypeDto>> GetActiveAssetTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.AssetTypes
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Code)
            .Select(x => new AssetTypeDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                ManagedByDepartmentId = x.ManagedByDepartmentId,
                ManagedByDepartmentCode = x.ManagedByDepartment != null
                    ? x.ManagedByDepartment.Code
                    : string.Empty,
                IdentifierPolicy = x.IdentifierPolicy.ToString(),
                CanBeAssignedToCourse = x.CanBeAssignedToCourse,
                IsReturnable = x.IsReturnable,
                ShowInCalendar = x.ShowInCalendar,
                IsActive = x.IsActive,
                SortOrder = x.SortOrder,
                Icon = x.Icon,
                Color = x.Color,
                Variants = x.Variants
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.SortOrder)
                    .ThenBy(v => v.Code)
                    .Select(v => new AssetVariantDto
                    {
                        Id = v.Id,
                        Code = v.Code,
                        Name = v.Name,
                        Description = v.Description,
                        IsActive = v.IsActive,
                        SortOrder = v.SortOrder
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }
}