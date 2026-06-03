using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tams.Api.Domain.Entities;
using Tams.Api.Domain.Enums;

namespace Tams.Api.Infrastructure.Data.Configurations;

public sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("devices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.AssetVariantId)
            .HasColumnName("asset_variant_id");

        builder.Property(x => x.Uid)
            .HasColumnName("uid")
            .HasMaxLength(100);

        builder.Property(x => x.Barcode)
            .HasColumnName("barcode")
            .HasMaxLength(100);

        builder.Property(x => x.SerialNumber)
            .HasColumnName("serial_number")
            .HasMaxLength(150);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(DeviceStatus.Available)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        builder.HasOne(x => x.AssetVariant)
            .WithMany()
            .HasForeignKey(x => x.AssetVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Name);

        builder.HasIndex(x => x.AssetVariantId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Uid)
            .IsUnique()
            .HasFilter("uid IS NOT NULL");

        builder.HasIndex(x => x.Barcode)
            .IsUnique()
            .HasFilter("barcode IS NOT NULL");

        builder.HasIndex(x => x.SerialNumber)
            .IsUnique()
            .HasFilter("serial_number IS NOT NULL");
    }
}