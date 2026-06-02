using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Tams.Api.Infrastructure.Data;

public sealed class TamsDbContextFactory : IDesignTimeDbContextFactory<TamsDbContext>
{
    public TamsDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Development";

        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("TamsDb")
            ?? throw new InvalidOperationException(
                "Connection string 'TamsDb' was not found. Configure it with user-secrets or environment variables.");

        var optionsBuilder = new DbContextOptionsBuilder<TamsDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new TamsDbContext(optionsBuilder.Options);
    }
}