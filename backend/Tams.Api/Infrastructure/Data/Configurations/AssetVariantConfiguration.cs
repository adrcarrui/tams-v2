using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tams.Api.Domain.Entities;

namespace Tams.Api.Infrastructure.Data.Configurations;

public sealed class AssetVariantConfiguration : IEntityTypeConfiguration<AssetVariant>
{
    public void Configure(EntityTypeBuilder<AssetVariant> builder)
    {
        builder.ToTable("asset_variants");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.AssetTypeId)
            .HasColumnName("asset_type_id");

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

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(x => x.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        builder.HasOne(x => x.AssetType)
            .WithMany(x => x.Variants)
            .HasForeignKey(x => x.AssetTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.AssetTypeId, x.Code })
            .IsUnique();

        builder.HasData(
            new AssetVariant
            {
                Id = 1,
                AssetTypeId = 1,
                Code = "VENDING",
                Name = "Vending",
                Description = "Cards physically assigned to vending use.",
                IsActive = true,
                SortOrder = 10,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AssetVariant
            {
                Id = 2,
                AssetTypeId = 1,
                Code = "CANTEEN",
                Name = "Canteen",
                Description = "Cards physically assigned to canteen use.",
                IsActive = true,
                SortOrder = 20,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AssetVariant
            {
                Id = 3,
                AssetTypeId = 1,
                Code = "INSTRUCTOR",
                Name = "Instructor",
                Description = "Cards physically assigned to instructors.",
                IsActive = true,
                SortOrder = 30,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AssetVariant
            {
                Id = 4,
                AssetTypeId = 1,
                Code = "GUEST",
                Name = "Guest",
                Description = "Cards physically assigned to guests.",
                IsActive = true,
                SortOrder = 40,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AssetVariant
            {
                Id = 5,
                AssetTypeId = 2,
                Code = "G8",
                Name = "G8",
                Description = "Laptop model G8.",
                IsActive = true,
                SortOrder = 10,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AssetVariant
            {
                Id = 6,
                AssetTypeId = 2,
                Code = "G10",
                Name = "G10",
                Description = "Laptop model G10.",
                IsActive = true,
                SortOrder = 20,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AssetVariant
            {
                Id = 7,
                AssetTypeId = 2,
                Code = "G11",
                Name = "G11",
                Description = "Laptop model G11.",
                IsActive = true,
                SortOrder = 30,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AssetVariant
            {
                Id = 8,
                AssetTypeId = 3,
                Code = "STANDARD",
                Name = "Standard",
                Description = "Standard USB device.",
                IsActive = true,
                SortOrder = 10,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
    }
}