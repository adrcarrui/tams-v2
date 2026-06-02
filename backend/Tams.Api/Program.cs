using Microsoft.EntityFrameworkCore;
using Tams.Api.Application.Departments;
using Tams.Api.Infrastructure.Data;
using Tams.Api.Application.AssetTypes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<AssetTypeService>();

var connectionString = builder.Configuration.GetConnectionString("TamsDb")
    ?? throw new InvalidOperationException("Connection string 'TamsDb' was not found.");

builder.Services.AddDbContext<TamsDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<DepartmentService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new
{
    app = "TAMS API",
    status = "running",
    health = "/api/health"
}));

app.MapControllers();

app.Run();

public partial class Program
{
}