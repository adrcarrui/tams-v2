using Microsoft.EntityFrameworkCore;
using Tams.Api.Contracts.Common;
using Tams.Api.Contracts.Devices;
using Tams.Api.Domain.Enums;
using Tams.Api.Infrastructure.Data;

namespace Tams.Api.Application.Devices;

public sealed class DeviceService
{
    private const int MaxPageSize = 100;

    private readonly TamsDbContext _dbContext;

    public DeviceService(TamsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResultDto<DeviceDto>> GetDevicesAsync(
        GetDevicesRequest request,
        CancellationToken cancellationToken = default)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : Math.Min(request.PageSize, MaxPageSize);

        var query = _dbContext.Devices
            .AsNoTracking()
            .AsQueryable();

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();

            query = query.Where(x =>
                x.Name.ToLower().Contains(search) ||
                (x.Uid != null && x.Uid.ToLower().Contains(search)) ||
                (x.Barcode != null && x.Barcode.ToLower().Contains(search)) ||
                (x.SerialNumber != null && x.SerialNumber.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<DeviceStatus>(request.Status, ignoreCase: true, out var parsedStatus))
            {
                throw new ArgumentException($"Invalid device status '{request.Status}'.");
            }

            query = query.Where(x => x.Status == parsedStatus);
        }

        if (request.AssetVariantId.HasValue)
        {
            query = query.Where(x => x.AssetVariantId == request.AssetVariantId.Value);
        }

        if (request.AssetTypeId.HasValue)
        {
            query = query.Where(x =>
                x.AssetVariant != null &&
                x.AssetVariant.AssetTypeId == request.AssetTypeId.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DeviceDto
            {
                Id = x.Id,
                Name = x.Name,
                AssetVariantId = x.AssetVariantId,
                AssetVariantCode = x.AssetVariant != null
                    ? x.AssetVariant.Code
                    : string.Empty,
                AssetVariantName = x.AssetVariant != null
                    ? x.AssetVariant.Name
                    : string.Empty,
                AssetTypeId = x.AssetVariant != null
                    ? x.AssetVariant.AssetTypeId
                    : 0,
                AssetTypeCode = x.AssetVariant != null && x.AssetVariant.AssetType != null
                    ? x.AssetVariant.AssetType.Code
                    : string.Empty,
                AssetTypeName = x.AssetVariant != null && x.AssetVariant.AssetType != null
                    ? x.AssetVariant.AssetType.Name
                    : string.Empty,
                ManagedByDepartmentId = x.AssetVariant != null && x.AssetVariant.AssetType != null
                    ? x.AssetVariant.AssetType.ManagedByDepartmentId
                    : 0,
                ManagedByDepartmentCode = x.AssetVariant != null &&
                                          x.AssetVariant.AssetType != null &&
                                          x.AssetVariant.AssetType.ManagedByDepartment != null
                    ? x.AssetVariant.AssetType.ManagedByDepartment.Code
                    : string.Empty,
                Uid = x.Uid,
                Barcode = x.Barcode,
                SerialNumber = x.SerialNumber,
                Status = x.Status.ToString(),
                IsActive = x.IsActive,
                Notes = x.Notes,
                CreatedAtUtc = x.CreatedAtUtc,
                UpdatedAtUtc = x.UpdatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<DeviceDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }
}