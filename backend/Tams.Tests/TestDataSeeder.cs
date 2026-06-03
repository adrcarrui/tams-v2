using Tams.Api.Domain.Entities;
using Tams.Api.Domain.Enums;
using Tams.Api.Infrastructure.Data;

namespace Tams.Tests;

public static class TestDataSeeder
{
    public static void SeedBaseData(TamsDbContext dbContext)
    {
        var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        dbContext.Departments.AddRange(
            new Department
            {
                Id = 1,
                Code = "TCO",
                Name = "TCO",
                IsActive = true,
                CreatedAtUtc = createdAt
            },
            new Department
            {
                Id = 2,
                Code = "ITC",
                Name = "ITC Support",
                IsActive = true,
                CreatedAtUtc = createdAt
            });

        dbContext.AssetTypes.AddRange(
            new AssetType
            {
                Id = 1,
                Code = "CARD",
                Name = "Card",
                ManagedByDepartmentId = 1,
                IdentifierPolicy = AssetIdentifierPolicy.Rfid,
                CanBeAssignedToCourse = true,
                IsReturnable = true,
                ShowInCalendar = true,
                IsActive = true,
                SortOrder = 10,
                CreatedAtUtc = createdAt
            },
            new AssetType
            {
                Id = 2,
                Code = "LAPTOP",
                Name = "Laptop",
                ManagedByDepartmentId = 2,
                IdentifierPolicy = AssetIdentifierPolicy.Barcode,
                CanBeAssignedToCourse = true,
                IsReturnable = true,
                ShowInCalendar = true,
                IsActive = true,
                SortOrder = 20,
                CreatedAtUtc = createdAt
            },
            new AssetType
            {
                Id = 3,
                Code = "USB",
                Name = "USB",
                ManagedByDepartmentId = 2,
                IdentifierPolicy = AssetIdentifierPolicy.Barcode,
                CanBeAssignedToCourse = true,
                IsReturnable = true,
                ShowInCalendar = true,
                IsActive = true,
                SortOrder = 30,
                CreatedAtUtc = createdAt
            });

        dbContext.AssetVariants.AddRange(
            new AssetVariant
            {
                Id = 1,
                AssetTypeId = 1,
                Code = "VENDING",
                Name = "Vending",
                IsActive = true,
                SortOrder = 10,
                CreatedAtUtc = createdAt
            },
            new AssetVariant
            {
                Id = 2,
                AssetTypeId = 1,
                Code = "CANTEEN",
                Name = "Canteen",
                IsActive = true,
                SortOrder = 20,
                CreatedAtUtc = createdAt
            },
            new AssetVariant
            {
                Id = 6,
                AssetTypeId = 2,
                Code = "G10",
                Name = "G10",
                IsActive = true,
                SortOrder = 20,
                CreatedAtUtc = createdAt
            },
            new AssetVariant
            {
                Id = 8,
                AssetTypeId = 3,
                Code = "STANDARD",
                Name = "Standard",
                IsActive = true,
                SortOrder = 10,
                CreatedAtUtc = createdAt
            });

        dbContext.SaveChanges();
    }
}