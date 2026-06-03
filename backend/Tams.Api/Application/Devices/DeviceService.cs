using Microsoft.EntityFrameworkCore;
using Tams.Api.Application.Common;
using Tams.Api.Contracts.Common;
using Tams.Api.Contracts.Devices;
using Tams.Api.Domain.Entities;
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
            .Select(x => ToDeviceDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<DeviceDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<DeviceDto?> GetDeviceByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Devices
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => ToDeviceDto(x))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ServiceResult<DeviceDto>> CreateDeviceAsync(
        CreateDeviceRequest request,
        CancellationToken cancellationToken = default)
    {
        var name = NormalizeOptionalText(request.Name);
        var uid = NormalizeOptionalText(request.Uid);
        var barcode = NormalizeOptionalText(request.Barcode);
        var serialNumber = NormalizeOptionalText(request.SerialNumber);
        var notes = NormalizeOptionalText(request.Notes);

        if (string.IsNullOrWhiteSpace(name))
        {
            return ServiceResult<DeviceDto>.Failure(
                "DEVICE_NAME_REQUIRED",
                "Device name is required.");
        }

        if (request.AssetVariantId <= 0)
        {
            return ServiceResult<DeviceDto>.Failure(
                "DEVICE_ASSET_VARIANT_REQUIRED",
                "Asset variant is required.");
        }

        var assetVariant = await _dbContext.AssetVariants
            .AsNoTracking()
            .Include(x => x.AssetType)
            .FirstOrDefaultAsync(
                x => x.Id == request.AssetVariantId,
                cancellationToken);

        if (assetVariant is null)
        {
            return ServiceResult<DeviceDto>.Failure(
                "ASSET_VARIANT_NOT_FOUND",
                "Asset variant was not found.");
        }

        if (!assetVariant.IsActive)
        {
            return ServiceResult<DeviceDto>.Failure(
                "ASSET_VARIANT_INACTIVE",
                "Asset variant is inactive.");
        }

        if (assetVariant.AssetType is null)
        {
            return ServiceResult<DeviceDto>.Failure(
                "ASSET_TYPE_NOT_FOUND",
                "Asset type was not found.");
        }

        if (!assetVariant.AssetType.IsActive)
        {
            return ServiceResult<DeviceDto>.Failure(
                "ASSET_TYPE_INACTIVE",
                "Asset type is inactive.");
        }

        var identifierValidation = ValidateIdentifierPolicy(
            assetVariant.AssetType.IdentifierPolicy,
            uid,
            barcode);

        if (!identifierValidation.IsSuccess)
        {
            return ServiceResult<DeviceDto>.Failure(
                identifierValidation.ErrorCode!,
                identifierValidation.ErrorMessage!);
        }

        var duplicateValidation = await ValidateUniqueIdentifiersAsync(
            uid,
            barcode,
            serialNumber,
            cancellationToken);

        if (!duplicateValidation.IsSuccess)
        {
            return ServiceResult<DeviceDto>.Failure(
                duplicateValidation.ErrorCode!,
                duplicateValidation.ErrorMessage!);
        }

        var device = new Device
        {
            Name = name,
            AssetVariantId = assetVariant.Id,
            Uid = uid,
            Barcode = barcode,
            SerialNumber = serialNumber,
            Status = DeviceStatus.Available,
            IsActive = true,
            Notes = notes,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.Devices.Add(device);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var createdDevice = await GetDeviceByIdAsync(device.Id, cancellationToken);

        if (createdDevice is null)
        {
            return ServiceResult<DeviceDto>.Failure(
                "DEVICE_CREATE_FAILED",
                "Device was created but could not be loaded.");
        }

        return ServiceResult<DeviceDto>.Success(createdDevice);
    }

    private static ServiceResult<bool> ValidateIdentifierPolicy(
        AssetIdentifierPolicy policy,
        string? uid,
        string? barcode)
    {
        var hasUid = !string.IsNullOrWhiteSpace(uid);
        var hasBarcode = !string.IsNullOrWhiteSpace(barcode);

        return policy switch
        {
            AssetIdentifierPolicy.None => ServiceResult<bool>.Success(true),

            AssetIdentifierPolicy.Rfid when !hasUid =>
                ServiceResult<bool>.Failure(
                    "DEVICE_UID_REQUIRED",
                    "UID is required for this asset type."),

            AssetIdentifierPolicy.Barcode when !hasBarcode =>
                ServiceResult<bool>.Failure(
                    "DEVICE_BARCODE_REQUIRED",
                    "Barcode is required for this asset type."),

            AssetIdentifierPolicy.RfidOrBarcode when !hasUid && !hasBarcode =>
                ServiceResult<bool>.Failure(
                    "DEVICE_IDENTIFIER_REQUIRED",
                    "UID or barcode is required for this asset type."),

            AssetIdentifierPolicy.RfidAndBarcode when !hasUid || !hasBarcode =>
                ServiceResult<bool>.Failure(
                    "DEVICE_UID_AND_BARCODE_REQUIRED",
                    "UID and barcode are required for this asset type."),

            _ => ServiceResult<bool>.Success(true)
        };
    }

    private async Task<ServiceResult<bool>> ValidateUniqueIdentifiersAsync(
        string? uid,
        string? barcode,
        string? serialNumber,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(uid))
        {
            var uidExists = await _dbContext.Devices
                .AsNoTracking()
                .AnyAsync(x => x.Uid == uid, cancellationToken);

            if (uidExists)
            {
                return ServiceResult<bool>.Failure(
                    "DEVICE_UID_ALREADY_EXISTS",
                    "Another device already uses this UID.");
            }
        }

        if (!string.IsNullOrWhiteSpace(barcode))
        {
            var barcodeExists = await _dbContext.Devices
                .AsNoTracking()
                .AnyAsync(x => x.Barcode == barcode, cancellationToken);

            if (barcodeExists)
            {
                return ServiceResult<bool>.Failure(
                    "DEVICE_BARCODE_ALREADY_EXISTS",
                    "Another device already uses this barcode.");
            }
        }

        if (!string.IsNullOrWhiteSpace(serialNumber))
        {
            var serialNumberExists = await _dbContext.Devices
                .AsNoTracking()
                .AnyAsync(x => x.SerialNumber == serialNumber, cancellationToken);

            if (serialNumberExists)
            {
                return ServiceResult<bool>.Failure(
                    "DEVICE_SERIAL_NUMBER_ALREADY_EXISTS",
                    "Another device already uses this serial number.");
            }
        }

        return ServiceResult<bool>.Success(true);
    }

    private static DeviceDto ToDeviceDto(Device device)
    {
        return new DeviceDto
        {
            Id = device.Id,
            Name = device.Name,
            AssetVariantId = device.AssetVariantId,
            AssetVariantCode = device.AssetVariant != null
                ? device.AssetVariant.Code
                : string.Empty,
            AssetVariantName = device.AssetVariant != null
                ? device.AssetVariant.Name
                : string.Empty,
            AssetTypeId = device.AssetVariant != null
                ? device.AssetVariant.AssetTypeId
                : 0,
            AssetTypeCode = device.AssetVariant != null && device.AssetVariant.AssetType != null
                ? device.AssetVariant.AssetType.Code
                : string.Empty,
            AssetTypeName = device.AssetVariant != null && device.AssetVariant.AssetType != null
                ? device.AssetVariant.AssetType.Name
                : string.Empty,
            ManagedByDepartmentId = device.AssetVariant != null && device.AssetVariant.AssetType != null
                ? device.AssetVariant.AssetType.ManagedByDepartmentId
                : 0,
            ManagedByDepartmentCode = device.AssetVariant != null &&
                                      device.AssetVariant.AssetType != null &&
                                      device.AssetVariant.AssetType.ManagedByDepartment != null
                ? device.AssetVariant.AssetType.ManagedByDepartment.Code
                : string.Empty,
            Uid = device.Uid,
            Barcode = device.Barcode,
            SerialNumber = device.SerialNumber,
            Status = device.Status.ToString(),
            IsActive = device.IsActive,
            Notes = device.Notes,
            CreatedAtUtc = device.CreatedAtUtc,
            UpdatedAtUtc = device.UpdatedAtUtc
        };
    }

    private static string? NormalizeOptionalText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}