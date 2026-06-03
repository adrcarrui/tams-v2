using Microsoft.EntityFrameworkCore;
using Tams.Api.Infrastructure.Data;

namespace Tams.Tests;

public static class TestDbContextFactory
{
    public static TamsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TamsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new TamsDbContext(options);

        TestDataSeeder.SeedBaseData(dbContext);

        return dbContext;
    }
}