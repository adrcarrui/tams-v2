using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tams.Api.Domain.Entities;
using Tams.Api.Domain.Enums;

namespace Tams.Api.Infrastructure.Data.Configurations;

public sealed class AssetTypeConfiguration : IEntityTypeConfiguration<AssetType>
{
    public void Configure(EntityTypeBuilder<AssetType> builder)
    {
        builder.ToTable("asset_types");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(x => x.ManagedByDepartmentId)
            .HasColumnName("managed_by_department_id");

        builder.Property(x => x.IdentifierPolicy)
            .HasColumnName("identifier_policy")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CanBeAssignedToCourse)
            .HasColumnName("can_be_assigned_to_course")
            .HasDefaultValue(true);

        builder.Property(x => x.IsReturnable)
            .HasColumnName("is_returnable")
            .HasDefaultValue(true);

        builder.Property(x => x.ShowInCalendar)
            .HasColumnName("show_in_calendar")
            .HasDefaultValue(true);

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(x => x.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue(0);

        builder.Property(x => x.Icon)
            .HasColumnName("icon")
            .HasMaxLength(100);

        builder.Property(x => x.Color)
            .HasColumnName("color")
            .HasMaxLength(50);

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasOne(x => x.ManagedByDepartment)
            .WithMany()
            .HasForeignKey(x => x.ManagedByDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new AssetType
            {
                Id = 1,
                Code = "CARD",
                Name = "Card",
                Description = "NFC card managed by TCO.",
                ManagedByDepartmentId = 1,
                IdentifierPolicy = AssetIdentifierPolicy.Rfid,
                CanBeAssignedToCourse = true,
                IsReturnable = true,
                ShowInCalendar = true,
                IsActive = true,
                SortOrder = 10,
                Icon = "credit-card",
                Color = "#0d6efd",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AssetType
            {
                Id = 2,
                Code = "LAPTOP",
                Name = "Laptop",
                Description = "Laptop managed by ITC Support.",
                ManagedByDepartmentId = 2,
                IdentifierPolicy = AssetIdentifierPolicy.Barcode,
                CanBeAssignedToCourse = true,
                IsReturnable = true,
                ShowInCalendar = true,
                IsActive = true,
                SortOrder = 20,
                Icon = "laptop",
                Color = "#198754",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AssetType
            {
                Id = 3,
                Code = "USB",
                Name = "USB",
                Description = "USB device managed by ITC Support.",
                ManagedByDepartmentId = 2,
                IdentifierPolicy = AssetIdentifierPolicy.Barcode,
                CanBeAssignedToCourse = true,
                IsReturnable = true,
                ShowInCalendar = true,
                IsActive = true,
                SortOrder = 30,
                Icon = "usb",
                Color = "#6f42c1",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
    }
}