using Microsoft.EntityFrameworkCore;
using Tams.Api.Domain.Entities;

namespace Tams.Api.Infrastructure.Data;

public sealed class TamsDbContext : DbContext
{
    public TamsDbContext(DbContextOptions<TamsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<AssetType> AssetTypes => Set<AssetType>();

    public DbSet<AssetVariant> AssetVariants => Set<AssetVariant>();

    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TamsDbContext).Assembly);
    }
}