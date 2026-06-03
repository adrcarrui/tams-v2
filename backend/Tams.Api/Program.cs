using Microsoft.EntityFrameworkCore;
using Tams.Api.Application.AssetTypes;
using Tams.Api.Application.Departments;
using Tams.Api.Application.Devices;
using Tams.Api.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "TamsFrontend";

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("TamsDb")
    ?? throw new InvalidOperationException("Connection string 'TamsDb' was not found.");

builder.Services.AddDbContext<TamsDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<AssetTypeService>();
builder.Services.AddScoped<DeviceService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

app.MapControllers();

app.Run();

public partial class Program
{
}